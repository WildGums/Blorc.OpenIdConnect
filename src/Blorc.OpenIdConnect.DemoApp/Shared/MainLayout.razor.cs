namespace Blorc.OpenIdConnect.DemoApp.Shared
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;

    public partial class MainLayout
    {
        public Models.User<Models.Profile> User { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (UserManager is not null)
            {
                User = await UserManager.GetUserAsync<Models.User<Models.Profile>>(AuthenticationStateTask);
            }
        }
    }
}
