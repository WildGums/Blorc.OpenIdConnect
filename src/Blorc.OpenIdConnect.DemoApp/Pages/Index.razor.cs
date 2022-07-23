namespace Blorc.OpenIdConnect.DemoApp.Pages
{
    using System.Threading.Tasks;
    using Blorc.OpenIdConnect.DemoApp;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;

    public sealed partial class Index : IDisposable
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (UserManager is null)
            {
                return;
            }

            User = await UserManager.GetUserAsync<User<Profile>>();

            UserManager.UserActivity += OnUserManagerUserActivity;
            UserManager.UserInactivity += OnUserManagerUserInactivity;
        }

        private void OnUserManagerUserInactivity(object sender, UserInactivityEventArgs args)
        {
            SignoutTimeSpan = args.SignoutTimeSpan;
            StateHasChanged();
        }

        private void OnUserManagerUserActivity(object sender, EventArgs e)
        {
            SignoutTimeSpan = null;
            StateHasChanged();
        }

        public TimeSpan? SignoutTimeSpan { get; set; }

        public User<Profile> User { get; set; }

        private async Task OnLoginButtonClickAsync(MouseEventArgs obj)
        {
            await UserManager.SigninRedirectAsync();
        }

        private async Task OnLogoutButtonClickAsync(MouseEventArgs obj)
        {
            await UserManager.SignoutRedirectAsync();
        }

        public void Dispose()
        {
            UserManager.UserActivity -= OnUserManagerUserActivity;
            UserManager.UserInactivity -= OnUserManagerUserInactivity;
        }
    }
}
