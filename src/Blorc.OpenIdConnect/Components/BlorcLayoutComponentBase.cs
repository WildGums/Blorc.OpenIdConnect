namespace Blorc.OpenIdConnect.Components
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;

    public class BlorcLayoutComponentBase : Blorc.Components.BlorcLayoutComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (AuthenticationStateTask is not null)
            {
                await AuthenticationStateTask;
            }
        }
    }
}
