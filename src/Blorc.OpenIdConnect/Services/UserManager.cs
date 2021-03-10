namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Blorc.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public class UserManager : IUserManager
    {
        private readonly IConfigurationService _configurationService;

        private readonly IJSRuntime _jsRuntime;

        private readonly NavigationManager _navigationManager;

        private OidcProviderOptions _options;

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, IConfigurationService configurationService, OidcProviderOptions options)
            : this(jsRuntime, navigationManager, configurationService)
        {
            _options = options;
        }

        public UserManager(IJSRuntime jsRuntime, NavigationManager navigationManager, IConfigurationService configurationService)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _configurationService = configurationService;
        }

        public Dictionary<string, string> Configuration { get; set; }

        public async Task<User> GetUserAsync()
        {
            if (await IsAuthenticatedAsync())
            {
                // TODO: Improve this condition
                var absoluteUri = _navigationManager.Uri;
                if (absoluteUri.Contains("state=") && absoluteUri.Contains("id_token=") && absoluteUri.Contains("access_token=") && absoluteUri.Contains("id_token=") && absoluteUri.Contains("token_type=bearer"))
                {
                    var absoluteUrlSplit = absoluteUri.Split('#');
                    var baseUri = absoluteUrlSplit.Length == 2 ? absoluteUrlSplit[0] : _navigationManager.BaseUri;
                    _navigationManager.NavigateTo(baseUri);
                }

                return await _jsRuntime.InvokeAsync<User>("BlorcOidc.Client.UserManager.GetUser");
            }

            return await Task.FromResult<User>(null);
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
            if (Configuration is null && _options is not null)
            {
                var serializedOptions = JsonSerializer.Serialize(_options);
                Configuration = JsonSerializer.Deserialize<Dictionary<string, string>>(serializedOptions);
            }

            if (Configuration is not null)
            {
                await InitializeAsync(() => Task.FromResult(Configuration));
            }
            else
            {
                await InitializeAsync(async () => await _configurationService.GetSectionAsync<Dictionary<string, string>>("identityserver"));
            }
        }

        private async Task<bool> IsInitialized()
        {
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsInitialized");
        }
    }
}
