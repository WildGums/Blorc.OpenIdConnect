using System.Net.Http.Headers;
using Blorc.OpenIdConnect;
using Blorc.OpenIdConnect.DemoApp;
using Blorc.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

var baseUrl = builder.HostEnvironment.BaseAddress;
builder.Services
    .AddHttpClient<WeatherForecastHttpClient>(client => client.BaseAddress = new Uri(baseUrl))
    // Add access token using this
    .AddAccessToken();

    // Or using this 
    // .CustomizeHttpRequestMessage(
    //    async (provider, request) =>
    //    {
    //        var userManager = provider.GetRequiredService<IUserManager>();
    //        var user = await userManager.GetUserAsync();
    //        if (user is not null)
    //        {
    //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
    //        }
    //    });

builder.Services.AddBlorcCore();
builder.Services.AddAuthorizationCore();

builder.Services.AddBlorcOpenIdConnect(
    options =>
    {
        builder.Configuration.Bind("IdentityServer", options);
        options.RedirectUri = baseUrl;
        options.PostLogoutRedirectUri = baseUrl;
        options.AutomaticSilentRenew = true;
        options.ResponseType = "code";
        options.FilterProtocolClaims = true;
        options.LoadUserInfo = true;
        options.Scope = "openid profile demo-api";
        options.TimeForUserInactivityAutomaticSignout = 15000;
        options.TimeForUserInactivityNotification = 10000;
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
