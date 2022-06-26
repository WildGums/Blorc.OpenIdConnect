namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Blorc.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.JSInterop;

    public class UserManager : IUserManager
    {
        private static readonly string[] ExpectedParameters = { "state", "session_state", "code", "access_token", "id_token", "token_type" };

        private readonly IJSRuntime _jsRuntime;

        private readonly NavigationManager _navigationManager;

        private readonly Dictionary<Type, object> _usersCache = new Dictionary<Type, object>();

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, OidcProviderOptions options)
            : this(jsRuntime, navigationManager)
        {
            var serializedOptions = JsonSerializer.Serialize(options);
            Configuration = JsonSerializer.Deserialize<Dictionary<string, object>>(serializedOptions);
        }

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        public Dictionary<string, object> Configuration { get; private set; }

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

        public async Task InitializeAsync(Func<Task<Dictionary<string, object>>> configurationResolver)
        {
            if (!await IsInitializedAsync())
            {
                await _jsRuntime.InvokeVoidAsync("BlorcOidc.Client.UserManager.Initialize", await configurationResolver());
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
            if (Configuration is not null)
            {
                await InitializeAsync(() => Task.FromResult(Configuration));
            }
        }

        private async Task<bool> IsInitializedAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsInitialized");
        }

        private async Task<bool> IsRedirectedAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Navigation.IsRedirected");
        }
    }
}
