namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect.Models;
    using Blorc.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.JSInterop;

    public class UserManager : IUserManager
    {
        private readonly IConfigurationService _configurationService;

        private readonly IJSRuntime _jsRuntime;

        private readonly NavigationManager _navigationManager;

        private User _user;

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, IConfigurationService configurationService, OidcProviderOptions options)
            : this(jsRuntime, navigationManager, configurationService)
        {
            var serializedOptions = JsonSerializer.Serialize(options);
            Configuration = JsonSerializer.Deserialize<Dictionary<string, string>>(serializedOptions);
        }

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, IConfigurationService configurationService)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _configurationService = configurationService;
        }

        public Dictionary<string, string> Configuration { get; private set; }

        public async Task<User> GetUserAsync(bool reload = true)
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

        private async Task<JsonElement?> GetUserJsonElementAsync()
        {
            if (await IsAuthenticatedAsync())
            {
                var absoluteUri = _navigationManager.Uri;
                if (absoluteUri.Contains("state=") && absoluteUri.Contains("id_token=") && absoluteUri.Contains("access_token=") && absoluteUri.Contains("id_token=") && absoluteUri.Contains("token_type=bearer"))
                {
                    var absoluteUrlSplit = absoluteUri.Split('#');
                    var baseUri = absoluteUrlSplit.Length == 2 ? absoluteUrlSplit[0] : _navigationManager.BaseUri;
                    _navigationManager.NavigateTo(baseUri);
                }

                return await _jsRuntime.InvokeAsync<JsonElement>("BlorcOidc.Client.UserManager.GetUser");
            }

            return null;
        }

        public async Task<User> GetUserAsync(Task<AuthenticationState> authenticationStateTask)
        {
            var authenticationState = await authenticationStateTask;
            if (authenticationState.User.Identity is not null && !authenticationState.User.Identity.IsAuthenticated)
            {
                return null;
            }

            return await GetUserAsync(false);
        }

        public async Task InitializeAsync(Func<Task<Dictionary<string, string>>> configurationResolver)
        {
            if (!await IsInitialized())
            {
                await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.Initialize", await configurationResolver());
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

        private async Task InitializeAsync()
        {
            if (Configuration is null)
            {
                Configuration = await _configurationService.GetSectionAsync<Dictionary<string, string>>("identityserver");
            }

            if (Configuration is not null)
            {
                await InitializeAsync(() => Task.FromResult(Configuration));
            }
        }

        private async Task<bool> IsInitialized()
        {
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsInitialized");
        }
    }
}
