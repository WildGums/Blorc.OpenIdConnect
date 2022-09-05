namespace Blorc.OpenIdConnect
{
    using System.Threading.Tasks;

    using Blorc.Services;

    public static class DocumentServiceExtensions
    {
        public static async Task InjectOpenIdConnectAsync(this IDocumentService documentService)
        {
            await documentService.InjectAssemblyScriptFileAsync(typeof(DocumentServiceExtensions).Assembly, "oidc-client.min.js");
            await documentService.InjectAssemblyScriptFileAsync(typeof(DocumentServiceExtensions).Assembly, "oidc-client-interop.js");
        }
    }
}
