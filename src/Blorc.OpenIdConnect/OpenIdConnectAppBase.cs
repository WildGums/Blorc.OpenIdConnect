namespace Blorc.OpenIdConnect
{
    using System.Threading.Tasks;

    using Blorc.Components;
    using Blorc.OpenIdConnect.Services.Extensions;
    using Blorc.Services;

    /// <summary>
    ///     The open id connect app base.
    /// </summary>
    public class OpenIdConnectAppBase : BlorcApplicationBase
    {
        /// <summary>
        ///     The on configuring document.
        /// </summary>
        /// <param name="documentService">
        ///     The document service.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        protected override async Task OnConfiguringDocumentAsync(IDocumentService documentService)
        {
            await documentService.InjectOpenIdConnect();
        }
    }
}
