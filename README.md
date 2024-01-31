# Blorc.OpenIdConnect

The right way to use OpenIdConnect on Blazor.

## Usage

1) Install Blorc.OpenIdConnect via NuGet.

2) Include `Blorc.Core/injector.js` the index.html file:

```html
<head>
    <!-- ... -->
    <script src="_content/Blorc.Core/injector.js"></script>
    <!-- ... -->
</head>
```
 
3) Update App.razor content like this:

```razor
@using Microsoft.AspNetCore.Components.Authorization

<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(Program).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
        <NotFound>
            <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```
        
4) Add the required service and update Program.cs file as follow:

```csharp
// Add access token delegating handler to registered http clients
var baseUrl = builder.HostEnvironment.BaseAddress;
builder.Services
    .AddHttpClient<WeatherForecastHttpClient>(client => client.BaseAddress = new Uri(baseUrl))
    .AddAccessToken();

// Registering required services
builder.Services.AddBlorcCore();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlorcOpenIdConnect(
    options =>
    {
        builder.Configuration.Bind("IdentityServer", options);
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
```

5) Configure the client and identity server as described in the [Configuration](#configuration) section.

## Configuration

### Client

Add a configuration file `wwwroot\appsettings.json`

> [!NOTE]
> The Authorization Code Flow with Proof Key for Code Exchange (PKCE) is an authorization code flow to prevent CSRF and authorization code injection attacks and it is the only supported method. Use the configuration values as shown below.

```jsonc
{
  "IdentityServer": {
    "ResponseType": "code",
    "Scope": "openid profile %API-NAME%",
    "RedirectUri": "%APPLICATION_URL%",
    "PostLogoutRedirectUri": "%APPLICATION_URL%",
    "Authority": "%IDENTITY_SERVER_URL%",
    "ClientId": "%CLIENT_ID%",
    "AutomaticSilentRenew": true, // or `false`
    "FilterProtocolClaims": true,
    "LoadUserInfo": true
  }
}
```

You can also configure the client when registering the service:

```csharp
builder.Services.AddBlorcOpenIdConnect(
    options =>
    {
        options.ResponseType = "code";
        // ...
    });
```

> Configuration sample code can be found in the [demo app](src/Blorc.OpenIdConnect.DemoApp/Program.cs).

Some of the configuration options are described in the following table:

| Option | Description |
|--------|-------------|
| LoadUserInfo | Flag to control if additional identity data is loaded from the user info endpoint in order to populate the user's profile. |
| Resource | The `resource` parameter to send to the identity server. Useful when the identity server supports [RFC 8707](https://datatracker.ietf.org/doc/html/rfc8707). |
| ExtraQueryParams | Additional query string parameters to be including in the authorization request. |
| ExtraTokenParams | Additional parameters to be sent to the token endpoint. |

### Identity server

Use the following guides as reference for identity server configuration.  

- [Authorization Code flow with PKCE (Keycloak)](https://www.appsdeveloperblog.com/pkce-verification-in-authorization-code-grant/)
- [PKCE Verification in Authorization Code Grant (Auth0)](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow-with-proof-key-for-code-exchange-pkce)


## Run demo app

1) Prerequisites

   - Docker
   - [Tye](https://github.com/dotnet/tye)

2) Open a command line console a run the following commands

```bash
> cd %CLONE_DIR%\deployment\tye
> tye run .\backend-tye.yaml
```

3) Run the InitializeKeycloakAsync test of the Environment class in the test project. This will setup the required clients, and client scope for the demo.

4) Run the `Blorc.OpenIdConnect.DemoApp.Server` project.

5) Use the following credentials when prompted by Keycloak.

| UserName | Password     |
|----------|--------------|
| admin    | Password123! |
