namespace Blorc.OpenIdConnect.DemoApp.Shared
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;

    public partial class MainLayout
    {
        public User User { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (UserManager is not null)
            {
                User = await UserManager.GetUserAsync();
            }
        }
    }
}
