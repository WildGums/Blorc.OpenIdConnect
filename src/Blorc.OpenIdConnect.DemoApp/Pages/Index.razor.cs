namespace Blorc.OpenIdConnect.DemoApp.Pages
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;

    public partial class Index
    {
        [Inject]
        public IUserManager UserManager { get; set; }

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
