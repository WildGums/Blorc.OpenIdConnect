using System.Net.Http.Headers;
using Blorc.OpenIdConnect;
using Blorc.OpenIdConnect.DemoApp;
using Blorc.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog.Extensions.Logging;
using Serilog;
using Serilog.Core;

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

// Logging
var levelSwitch = new LoggingLevelSwitch
{
#if DEBUG
    MinimumLevel = Serilog.Events.LogEventLevel.Debug
#else
    MinimumLevel = Serilog.Events.LogEventLevel.Information
#endif
};

var logger = Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
    .WriteTo.BrowserConsole()
    .CreateLogger();

builder.Services.AddSingleton<ILoggerFactory>(new SerilogLoggerFactory(logger, false));
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

var webAssemblyHost = builder.Build();

await webAssemblyHost
    .ConfigureDocumentAsync(
        async documentService =>
        {
            await documentService.InjectBlorcCoreJsAsync();
            await documentService.InjectOpenIdConnectAsync();
        });

await webAssemblyHost.RunAsync();
