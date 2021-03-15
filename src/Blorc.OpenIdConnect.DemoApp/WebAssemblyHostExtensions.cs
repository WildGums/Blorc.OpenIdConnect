namespace Blorc.OpenIdConnect.DemoApp
{
    using System;
    using System.Threading.Tasks;

    using Blorc.Services;

    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    // TODO: Remove this after update of Blorc.Core.
    public static class WebAssemblyHostExtensions
    {
        public static async Task ConfigureAsync(this WebAssemblyHost @this, Func<IServiceProvider, Task> options)
        {
            var requiredService = @this.Services.GetRequiredService<IServiceProvider>();
            await options(requiredService);
        }

        public static async Task ConfigureDocumentAsync(this WebAssemblyHost @this, Func<IDocumentService, Task> options)
        {
            await @this.ConfigureAsync(
                async provider =>
                {
                    var requiredService = @this.Services.GetRequiredService<IDocumentService>();
                    await options(requiredService);
                });
        }
    }
}
