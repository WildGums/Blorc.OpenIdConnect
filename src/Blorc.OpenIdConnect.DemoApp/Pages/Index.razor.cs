namespace Blorc.OpenIdConnect.DemoApp.Pages
{
    using System.Threading.Tasks;
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
            SignOutTimeSpan = args.SignOutTimeSpan;
            StateHasChanged();
        }

        private void OnUserManagerUserActivity(object sender, UserActivityEventArgs args)
        {
            SignOutTimeSpan = null;
            StateHasChanged();
        }

        public TimeSpan? SignOutTimeSpan { get; set; }

        public User<Profile> User { get; set; }

        private async Task OnLoginButtonClickAsync(MouseEventArgs obj)
        {
            await UserManager.SignInRedirectAsync();
        }

        private async Task OnLoginAndRedirectFetchDataButtonClickAsync(MouseEventArgs obj)
        {
            await UserManager.SignInRedirectAsync("/fetchdata");
        }

        private async Task OnLogoutButtonClickAsync(MouseEventArgs obj)
        {
            await UserManager.SignOutRedirectAsync();
        }

        public void Dispose()
        {
            UserManager.UserActivity -= OnUserManagerUserActivity;
            UserManager.UserInactivity -= OnUserManagerUserInactivity;
        }
    }
}
