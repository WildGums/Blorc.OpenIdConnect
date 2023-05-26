namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Blorc.OpenIdConnect.JSInterop;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.Extensions.Logging;
    using Microsoft.JSInterop;

    public class UserManager : IUserManager
    {
        private readonly OidcProviderOptions _options;

        private static readonly string[] ExpectedParameters = { "state", "session_state", "code", "access_token", "id_token", "token_type" };

        private readonly ILogger<UserManager> _logger;
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;

        private readonly Dictionary<Type, object?> _usersCache = new Dictionary<Type, object?>();

        private DotNetObjectReference<UserManager>? _objRef;

        private bool _disposed;
        private bool _isInitializing;

        private DateTime? _inactivityStartTime;

        public UserManager(ILogger<UserManager> logger, IJSRuntime jsRuntime, NavigationManager navigationManager, OidcProviderOptions options)
        {
            ArgumentNullException.ThrowIfNull(jsRuntime);
            ArgumentNullException.ThrowIfNull(navigationManager);
            ArgumentNullException.ThrowIfNull(options);
            _logger = logger;
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _options = options;
        }

        public event EventHandler<UserActivityEventArgs>? UserActivity;

        public event EventHandler<UserInactivityEventArgs>? UserInactivity;

        public virtual async Task<TUser?> GetUserAsync<TUser>(Task<AuthenticationState> authenticationStateTask, JsonSerializerOptions? options = null)
        {
            var authenticationState = await authenticationStateTask;
            if (authenticationState?.User?.Identity is not null && !authenticationState.User.Identity.IsAuthenticated)
            {
                return default;
            }

            return await GetUserAsync<TUser>(options: options);
        }

        public virtual async Task<TUser?> GetUserAsync<TUser>(bool reload = true, JsonSerializerOptions? options = null)
        {
            var userType = typeof(TUser);

            if (reload)
            {
                _usersCache.Remove(userType, out _);
            }
            else if (_usersCache.TryGetValue(userType, out var value) &&
                     value is TUser cacheUser)
            {
                return cacheUser;
            }

            var userJsonElement = await GetUserJsonElementAsync();
            if (userJsonElement.HasValue)
            {
                var user = userJsonElement.Value.ToObject<TUser>(options);
                _usersCache[userType] = user;
                return user;
            }

            return default;
        }

        public virtual async Task InitializeAsync(Func<Task<Dictionary<string, JsonElement>>> configurationResolver)
        {
            ArgumentNullException.ThrowIfNull(configurationResolver);

            // Max wait time is 5 seconds
            var safetyCounter = 1000;
            while (_isInitializing &&
                safetyCounter-- >= 0)
            {
                await Task.Delay(50);
            }

            if (await IsInitializedAsync())
            {
                return;
            }

            try
            {
                _isInitializing = true;

                _logger.LogDebug("Initializing user manager");

                _objRef?.Dispose();
                _objRef = DotNetObjectReference.Create(this);

                await _jsRuntime.InvokeVoidAsync("BlorcOidc.Client.UserManager.Initialize", await configurationResolver(), _objRef);

                _logger.LogDebug("Initialized user manager");
            }
            finally
            {
                _isInitializing = false;
            }
        }

        public virtual async Task<bool> IsAuthenticatedAsync()
        {
            await InitializeAsync();

            var value = await InvokeWithPromiseHandlerAsync<bool?>(new PromiseHandlerContext("BlorcOidc.Client.UserManager.IsAuthenticated"), () => false);
            return value ?? false;
        }

        public virtual async Task<bool> SignInRedirectAsync(string redirectUri = "")
        {
            _logger.LogDebug("Signing in");

            if (!string.IsNullOrWhiteSpace(redirectUri))
            {
                var uri = new Uri(redirectUri, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                {
                    redirectUri = _navigationManager.ToAbsoluteUri(redirectUri).AbsoluteUri;
                }
            }

            var result = await InvokeWithPromiseHandlerAsync<bool>(new PromiseHandlerContext("BlorcOidc.Client.UserManager.SignInRedirect", new[] { redirectUri }));

            _logger.LogDebug("Sign in result: '{Result}'", result);

            return result;
        }

        public virtual async Task<bool> SignOutRedirectAsync()
        {
            _logger.LogDebug("Signing out");

            var result = await InvokeWithPromiseHandlerAsync<bool>(new PromiseHandlerContext("BlorcOidc.Client.UserManager.SignOutRedirect"));

            _logger.LogDebug("Sign out result: '{Result}'", result);

            return result;
        }

        [JSInvokable]
        public double OnUserInactivity()
        {
            var now = DateTime.Now;
            _inactivityStartTime ??= now.Subtract(_options.GetTimeForUserInactivityNotification());
            var elapsedTime = now.Subtract(_inactivityStartTime.Value);
            var remainingTime = _options.GetTimeForUserInactivityAutomaticSignOut() - elapsedTime;
            if (remainingTime <= TimeSpan.Zero)
            {
                _ = Task.Run(SignOutRedirectAsync);
            }

            var userInactivityEventArgs = new UserInactivityEventArgs(remainingTime);
            RaiseUserInactivity(userInactivityEventArgs);
            if (userInactivityEventArgs.InactivityNotificationTimeSpan is not null && userInactivityEventArgs.InactivityNotificationTimeSpan >= TimeSpan.Zero)
            {
                return Math.Min(remainingTime.TotalMilliseconds, userInactivityEventArgs.InactivityNotificationTimeSpan.Value.TotalMilliseconds);
            }

            return remainingTime.TotalMilliseconds;
        }

        [JSInvokable]
        public void OnUserActivity()
        {
            var userActivityEventArgs = new UserActivityEventArgs(_inactivityStartTime ?? DateTime.Now, DateTime.Now);
            RaiseUserActivity(userActivityEventArgs);
            if (userActivityEventArgs.ResetTime)
            {
                _inactivityStartTime = null;
            }
        }

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _objRef?.Dispose();

            _isInitializing = false;
            _disposed = true;
        }

        protected virtual async Task<JsonElement?> GetUserJsonElementAsync()
        {
            if (await IsAuthenticatedAsync())
            {
                if (!await IsRedirectedAsync())
                {
                    var getUserResponse = await InvokeWithPromiseHandlerAsync<JsonElement?>(new PromiseHandlerContext("BlorcOidc.Client.UserManager.GetUser"), () => null);
                    return getUserResponse;
                }

                var redirectRequired = false;
                var uri = new Uri(_navigationManager.Uri);
                var parameters = uri.Query.TrimStart('?').Split('&')
                    .Select(s => s.Split('='))
                    .Where(assignments => assignments.Length == 2)
                    .Select(assignments => (Name: assignments[0], Value: assignments[1]))
                    .ToList();

                for (var i = parameters.Count - 1; i >= 0; i--)
                {
                    if (ExpectedParameters.Contains(parameters[i].Name, StringComparer.InvariantCultureIgnoreCase))
                    {
                        parameters.RemoveAt(i);
                        redirectRequired = true;
                    }
                }

                if (redirectRequired)
                {
                    var url = new Uri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);
                    if (parameters.Count > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            if (parameter.Name is not null)
                            {
                                url += $"{parameter.Name}={parameter.Value}&";
                            }
                        }

                        url = url.TrimEnd('&');
                    }

                    _navigationManager.NavigateTo(url);
                }

                var jsonElement = await InvokeWithPromiseHandlerAsync<JsonElement?>(new PromiseHandlerContext("BlorcOidc.Client.UserManager.GetUser"), () => null);
                return jsonElement;
            }

            return null;
        }

        protected virtual async Task<T> InvokeWithPromiseHandlerAsync<T>(PromiseHandlerContext context)
        {
            var item = await _jsRuntime.InvokeWithPromiseHandlerAsync<T>(context);
            return item;
        }

        protected virtual async Task<T> InvokeWithPromiseHandlerAsync<T>(PromiseHandlerContext context, Func<T> defaultValue)
        {
            var item = await _jsRuntime.InvokeWithPromiseHandlerAsync<T>(context, defaultValue);
            return item;
        }

        protected virtual async Task InitializeAsync()
        {
            await InitializeAsync(() =>
            {
                _logger.LogDebug("Serializing options");

                var serializedOptions = JsonSerializer.Serialize(_options);

                _logger.LogDebug("Deserializing options");

                var deserializedOptions = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serializedOptions);
                if (deserializedOptions is null)
                {
                    throw new InvalidOperationException("Cannot initialize user manager");
                }

                _logger.LogDebug("Returning '{Count}' options", deserializedOptions.Count);

                return Task.FromResult(deserializedOptions);
            });
        }

        protected virtual async Task<bool> IsInitializedAsync()
        {
            var value = await _jsRuntime.InvokeWithPromiseHandlerAsync<bool>(new PromiseHandlerContext("BlorcOidc.Client.UserManager.IsInitialized")
            {
                // Should be super fast
                MaximumRetryCount = 1
            }, () => false);

            return value;
        }

        protected virtual async Task<bool> IsRedirectedAsync()
        {
            var value = await _jsRuntime.InvokeWithPromiseHandlerAsync<bool>(new PromiseHandlerContext("BlorcOidc.Navigation.IsRedirected"), () => false);
            return value;
        }

        protected virtual void RaiseUserInactivity(UserInactivityEventArgs e)
        {
            UserInactivity?.Invoke(this, e);
        }

        protected virtual void RaiseUserActivity(UserActivityEventArgs e)
        {
            UserActivity?.Invoke(this, e);
        }
    }
}
