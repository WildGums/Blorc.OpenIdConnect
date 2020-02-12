// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentServiceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2020 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Blorc.OpenIdConnect.Services.Extensions
{
    using System.Threading.Tasks;

    using Blorc.Services;

    public static class DocumentServiceExtensions
    {
        #region Methods
        public static async Task InjectOpenIdConnectJS(this IDocumentService documentService)
        {
            // Argument.IsNotNull(() => documentService);

            await documentService.InjectAssemblyScriptFile(typeof(DocumentServiceExtensions).Assembly, "oidc-client.min.js");
            await documentService.InjectAssemblyScriptFile(typeof(DocumentServiceExtensions).Assembly, "oidc-client-interop.js");
        }
        #endregion
    }
}
