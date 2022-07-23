namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.JSInterop;

    public sealed class UserManager : IUserManager
    {
        private readonly OidcProviderOptions _options;

        private static readonly string[] ExpectedParameters = { "state", "session_state", "code", "access_token", "id_token", "token_type" };

        private readonly IJSRuntime _jsRuntime;

        private readonly NavigationManager _navigationManager;

        private readonly Dictionary<Type, object> _usersCache = new Dictionary<Type, object>();

        private DotNetObjectReference<UserManager> _objRef;

        private bool _disposed;

        private DateTime? _inactivityStartTime;

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, OidcProviderOptions options)
            : this(jsRuntime, navigationManager)
        {
            _options = options;
        }

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        public event EventHandler<UserActivityEventArgs> UserActivity;

        public event EventHandler<UserInactivityEventArgs> UserInactivity;

        public async Task<TUser> GetUserAsync<TUser>(Task<AuthenticationState> authenticationStateTask, JsonSerializerOptions options = null)
        {
            var authenticationState = await authenticationStateTask;
            if (authenticationState?.User?.Identity is not null && !authenticationState.User.Identity.IsAuthenticated)
            {
                return default;
            }

            return await GetUserAsync<TUser>(options: options);
        }

        public async Task<TUser> GetUserAsync<TUser>(bool reload = true, JsonSerializerOptions options = null)
        {
            var userType = typeof(TUser);
            
            if (reload)
            {
                _usersCache.Remove(userType, out _);
            }
            else if (_usersCache.TryGetValue(userType,  out var value) && value is TUser cacheUser)
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

        public async Task InitializeAsync(Func<Task<Dictionary<string, JsonElement>>> configurationResolver)
        {
            if (!await IsInitializedAsync())
            {
                _objRef?.Dispose();
                _objRef = DotNetObjectReference.Create(this);
                await _jsRuntime.InvokeVoidAsync("BlorcOidc.Client.UserManager.Initialize", await configurationResolver(), _objRef);
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            await InitializeAsync();
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsAuthenticated");
        }

        public async Task SigninRedirectAsync()
        {
            await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.SigninRedirect");
        }

        public async Task SignoutRedirectAsync()
        {
            await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.SignoutRedirect");
        }

        [JSInvokable]
        public void OnUserInactivity()
        {
            var now = DateTime.Now;
            _inactivityStartTime ??= now.Subtract(_options.GetTimeForUserInactivityNotification());

            if (_inactivityStartTime is not null)
            {
                var elapsedTime = now.Subtract(_inactivityStartTime.Value);
                var remainingTime = _options.GetTimeForUserInactivityAutomaticLogout()  - elapsedTime;
                if (remainingTime <= TimeSpan.Zero)
                {
                    _ = Task.Run(SignoutRedirectAsync);
                }
                else
                {
                    OnUserInactivity(new UserInactivityEventArgs(remainingTime));
                }
            }
        }

        [JSInvokable]
        public void OnUserActivity()
        {
            var userActivityEventArgs = new UserActivityEventArgs(_inactivityStartTime ?? DateTime.Now, DateTime.Now);

            OnUserActivity(userActivityEventArgs);

            if (userActivityEventArgs.ResetTime)
            {
                _inactivityStartTime = null;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _objRef?.Dispose();

            _disposed = true;
        }

        private async Task<JsonElement?> GetUserJsonElementAsync()
        {
            if (await IsAuthenticatedAsync())
            {
                var absoluteUri = _navigationManager.Uri;
                if (ExpectedParameters.Any(parameter => absoluteUri.Contains($"{parameter}=", StringComparison.InvariantCultureIgnoreCase)) && await IsRedirectedAsync())
                {
                    var absoluteUrlSplit = absoluteUri.Split('#');
                    var baseUri = absoluteUrlSplit.Length == 2 ? absoluteUrlSplit[0] : _navigationManager.BaseUri;
                    _navigationManager.NavigateTo(baseUri);
                }

                return await _jsRuntime.InvokeAsync<JsonElement>("BlorcOidc.Client.UserManager.GetUser");
            }

            return null;
        }

        private async Task InitializeAsync()
        {
            await InitializeAsync(() => Task.FromResult(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(_options))));
        }

        private async Task<bool> IsInitializedAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsInitialized");
        }

        private async Task<bool> IsRedirectedAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Navigation.IsRedirected");
        }

        private void OnUserInactivity(UserInactivityEventArgs e)
        {
            UserInactivity?.Invoke(this, e);
        }

        private void OnUserActivity(UserActivityEventArgs e)
        {
            UserActivity?.Invoke(this, e);
        }
    }
}
