namespace Blorc.OpenIdConnect.DemoApp.Pages
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;

    public partial class Index
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (UserManager is not null)
            {
                User = await UserManager.GetUserAsync();
            }
        }

        public User User { get; set; }

        private async Task OnLoginButtonClick(MouseEventArgs obj)
        {
            await UserManager.SigninRedirectAsync();
        }

        private async Task OnLogoutButtonClick(MouseEventArgs obj)
        {
            await UserManager.SignoutRedirectAsync();
        }
    }
}
