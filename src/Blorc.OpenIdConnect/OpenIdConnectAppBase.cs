namespace Blorc.OpenIdConnect
{
    using System.Threading.Tasks;

    using Blorc.Components;
    using Blorc.OpenIdConnect.Services.Extensions;
    using Blorc.Services;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     The open id connect app base.
    /// </summary>
    public class OpenIdConnectAppBase : BlorcComponentBase
    {
        /// <summary>
        ///     Gets a value indicating whether initialized.
        /// </summary>
        protected bool Initialized { get; private set; }

        /// <summary>
        ///     Gets or sets the document service.
        /// </summary>
        [Inject]
        private IDocumentService DocumentService { get; set; }

        /// <summary>
        ///     The on initialized async.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await DocumentService.InjectOpenIdConnectJS();

            if (!Initialized)
            {
                Initialized = true;
                StateHasChanged();
            }
        }
    }
}
