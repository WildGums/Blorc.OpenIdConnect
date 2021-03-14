namespace Blorc.OpenIdConnect.Components
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class ImplicitFlowRefreshFrame : ComponentBase
    {
        [Inject]
        public IJSRuntime IJSRuntime { get; set; }

        [Parameter]
        public string RelativeSrc { get; set; } = "/protocol/openid-connect/login-status-iframe.html";

        public string Src
        {
            get
            {
                var authority = UserManager.Configuration["authority"];
                var clientId = UserManager.Configuration["client_id"];
                var redirectUrl = UserManager.Configuration["redirect_uri"].TrimEnd('/');

                // var s = UserManager.Configuration["authority"];
                // http://localhost:18080/auth/realms/master/protocol/openid-connect/login-status-iframe.html/init?client_id=demo-app&origin=http%3A%2F%2Flocalhost%3A5001
                // return authority + RelativeSrc + $"/init?client_id={WebUtility.UrlEncode(clientId)}&origin={WebUtility.UrlEncode(redirectUrl)}";
                return authority + RelativeSrc;
            }
        }

        [Inject]
        public IUserManager UserManager { get; set; }

        protected override void OnInitialized()
        {
            UserManager.AccessTokenExpiring += OnUserManagerAccessTokeExpiring;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await UserManager.GetUserAsync();
        }

        private void OnUserManagerAccessTokeExpiring(object sender, EventArgs eventArgs)
        {
            InvokeAsync(
                async () =>
                {
                    var clientId = UserManager.Configuration["client_id"];
                    var user = await UserManager.GetUserAsync();
                    await IJSRuntime.InvokeAsync<object>("window.postMessage", CancellationToken.None, $"{clientId} {user.SessionState}");
                });
        }
    }
}
