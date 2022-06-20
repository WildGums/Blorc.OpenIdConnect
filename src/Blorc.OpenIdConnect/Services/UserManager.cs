namespace Blorc.OpenIdConnect
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect.Models;
    using Blorc.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.JSInterop;

    public class UserManager : IUserManager
    {
        private static readonly string[] ExpectedParameters = { "state", "session_state", "access_token", "id_token", "token_type" };

        private readonly IConfigurationService _configurationService;

        private readonly IJSRuntime _jsRuntime;

        private readonly NavigationManager _navigationManager;

        private IUser _user;

        private readonly Dictionary<Type, object> _usersCache = new Dictionary<Type, object>();

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, IConfigurationService configurationService, OidcProviderOptions options)
            : this(jsRuntime, navigationManager, configurationService)
        {
            var serializedOptions = JsonSerializer.Serialize(options);
            Configuration = JsonSerializer.Deserialize<Dictionary<string, object>>(serializedOptions);
        }

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, IConfigurationService configurationService)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _configurationService = configurationService;
        }

        public Dictionary<string, object> Configuration { get; private set; }

        public async Task<TUser> GetUserAsync<TUser>(Task<AuthenticationState> authenticationStateTask, JsonSerializerOptions options = null)
        {
            var authenticationState = await authenticationStateTask;
            if (authenticationState.User.Identity is not null && !authenticationState.User.Identity.IsAuthenticated)
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
                var userJsonElement = await GetUserJsonElementAsync();
                if (userJsonElement.HasValue)
                {
                    var user = userJsonElement.Value.ToObject<TUser>(options);
                    _usersCache[userType] = user;
                    return user;
                }
            }

            if (!reload && _usersCache.TryGetValue(userType,  out var value) && value is TUser cacheUser)
            {
                return cacheUser;
            }

            return default;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "GetUserAsync")]
        public async Task<IUser> GetUserAsync(bool reload = true)
        {
            if (!reload && _user is not null)
            {
                return _user;
            }

            if (reload)
            {
                _user = null;
                var userJsonElement = await GetUserJsonElementAsync();
                if (userJsonElement.HasValue)
                {
                    _user = new User(userJsonElement.Value);
                }
            }

            return _user;
        }

        public async Task<IUser> GetUserAsync(Task<AuthenticationState> authenticationStateTask)
        {
            var authenticationState = await authenticationStateTask;
            if (authenticationState.User.Identity is not null && !authenticationState.User.Identity.IsAuthenticated)
            {
                return null;
            }

            return await GetUserAsync();
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
                if (ExpectedParameters.All(parameter => absoluteUri.Contains($"{parameter}=", StringComparison.InvariantCultureIgnoreCase)) && await IsRedirectedAsync())
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
            // TODO: Deprecate this.
            if (Configuration is null)
            {
                var configuration = await _configurationService.GetSectionAsync<Dictionary<string, string>>("identityserver");
                if (configuration is not null)
                {
                    Configuration = new Dictionary<string, object>();
                    foreach (var pair in configuration)
                    {
                        configuration[pair.Key] = pair.Value;
                    }
                }
            }

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
