namespace Blorc.OpenIdConnect.DemoApp.Shared
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;

    public partial class MainLayout
    {
        private TimeSpan? _signoutTimeSpan;

        public User<Profile> User { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (UserManager is not null)
            {
                User = await UserManager.GetUserAsync(AuthenticationStateTask);

                UserManager.UserActivity += (sender, args) =>
                {
                    _signoutTimeSpan = null;
                    Console.WriteLine("User activity");
                    StateHasChanged();
                };
                
                UserManager.UserInactivity += (sender, args) =>
                {
                    _signoutTimeSpan = args.SignoutTimeSpan;
                    if (_signoutTimeSpan <= TimeSpan.FromSeconds(5))
                    {
                        args.InactivityNotificationTimeSpan = TimeSpan.FromSeconds(1);
                    }

                    Console.WriteLine($"User inactivity, will be signed out in {_signoutTimeSpan}");
                    StateHasChanged();
                };
            }
        }
    }
}
