namespace Blorc.OpenIdConnect.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect.Models;
    using Blorc.OpenIdConnect.Services.Interfaces;
    using Blorc.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public class UserManager : IUserManager
    {
        #region Fields
        private readonly HttpClient _httpClient;

        private readonly IJSRuntime _jsRuntime;

        private readonly NavigationManager _navigationManager;

        private readonly IConfigurationService _configurationService;
        #endregion

        #region Constructors
        public UserManager(HttpClient httpClient, IJSRuntime jsRuntime, NavigationManager navigationManager, IConfigurationService configurationService)
        {
            // Argument.IsNotNull(() => httpClient);
            // Argument.IsNotNull(() => jsRuntime);
            // Argument.IsNotNull(() => navigationManager);
            // Argument.IsNotNull(() => configurationService);

            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _configurationService = configurationService;
        }

        #endregion

        #region Properties
        public Dictionary<string, string> Configuration { get; set; }
        #endregion

        #region IUserManager Members
        public async Task SigninRedirectAsync()
        {
            await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.SigninRedirect");
        }

        public async Task SignoutRedirectAsync()
        {
            await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.SignoutRedirect");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            await InitializeAsync();

            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsAuthenticated");
        }

        public async Task<User> GetUserAsync()
        {
            if (await IsAuthenticatedAsync())
            {
                // TODO: Improve this condition
                string absoluteUri = _navigationManager.Uri;
                if (absoluteUri.Contains("state=") && absoluteUri.Contains("id_token=") && absoluteUri.Contains("access_token=") && absoluteUri.Contains("id_token=") && absoluteUri.Contains("token_type=bearer"))
                {
                    var absoluteUrlSplitted = absoluteUri.Split('#');
                    var baseUri = absoluteUrlSplitted.Length == 2 ? absoluteUrlSplitted[0] : _navigationManager.BaseUri;
                    _navigationManager.NavigateTo(baseUri);
                }

                return await _jsRuntime.InvokeAsync<User>("BlorcOidc.Client.UserManager.GetUser");
            }

            return await Task.FromResult<User>(null);
        }

        #endregion

        #region Methods
        private async Task InitializeAsync()
        {
            if (Configuration != null)
            {
                await InitializeAsync(() => Task.FromResult(Configuration));
            }
            else
            {
                await InitializeAsync(async () => await _configurationService.GetSection<Dictionary<string, string>>("identityserver"));
            }
        }

        private async Task<bool> IsInitialized()
        {
            return await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsInitialized");
        }

        public async Task InitializeAsync(Func<Task<Dictionary<string, string>>> configurationResolver)
        {
            if (!await IsInitialized())
            {
                await _jsRuntime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.Initialize", await configurationResolver());
            }
        }

        #endregion
    }
}