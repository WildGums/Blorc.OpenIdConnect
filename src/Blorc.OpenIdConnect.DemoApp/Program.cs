namespace Blorc.OpenIdConnect.DemoApp
{
    using System;
    using System.Threading.Tasks;

    using Blorc.Services;

    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient<WeatherForecastHttpClient>(
                client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddBlorcCore();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlocOpenIdConnect(options => builder.Configuration.Bind("IdentityServer", options));

            var webAssemblyHost = builder.Build();

            var documentService = webAssemblyHost.Services.GetRequiredService<IDocumentService>();
            await documentService.InjectBlorcCoreJsAsync();
            await documentService.InjectOpenIdConnectAsync();

            await webAssemblyHost.RunAsync();
        }
    }
}
