namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            return await GetUserAsync(false);
        }

        public async Task InitializeAsync(Func<Task<Dictionary<string, string>>> configurationResolver)
        {
            if (!await IsInitializedAsync())
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
            if (Configuration is null)
            {
                Configuration = await _configurationService.GetSectionAsync<Dictionary<string, string>>("identityserver");
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
