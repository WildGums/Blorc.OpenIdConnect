using Blorc.OpenIdConnect;
using Blorc.OpenIdConnect.DemoApp;
using Blorc.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

var baseUrl = builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient<WeatherForecastHttpClient>(
    client => client.BaseAddress = new Uri(baseUrl));

builder.Services.AddBlorcCore();
builder.Services.AddAuthorizationCore();

builder.Services.AddBlocOpenIdConnect(
    options =>
    {
        builder.Configuration.Bind("IdentityServer", options);
        options.RedirectUri = baseUrl;
        options.PostLogoutRedirectUri = baseUrl;
        options.AutomaticSilentRenew = true;
    });



var webAssemblyHost = builder.Build();

await webAssemblyHost
    .ConfigureDocumentAsync(
        async documentService =>
        {
            await documentService.InjectBlorcCoreJsAsync();
            await documentService.InjectOpenIdConnectAsync();
        });

await webAssemblyHost.RunAsync();
