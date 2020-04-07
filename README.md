# Blorc.OpenIdConnect

## Usage

1) Install Blorc.OpenIdConnect via NuGet.

2) Include `Blorc.Core/injector.js` the index.html file:

        <head>
                ...
                <script src="_content/Blorc.Core/injector.js"></script>
                ...
        </head>        
 
3) Update App.razor content like this:

        @using Blorc.OpenIdConnect
        @using Microsoft.AspNetCore.Components.Authorization

        @inherits OpenIdConnectAppBase

        @if (Initialized)
        {
            <Router AppAssembly="@typeof(Program).Assembly">
                <Found Context="routeData">
                    <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
                </Found>
                <NotFound>
                    <CascadingAuthenticationState>
                        <LayoutView Layout="@typeof(MainLayout)">
                            <p>Sorry, there's nothing at this address.</p>
                        </LayoutView>
                    </CascadingAuthenticationState>
                </NotFound>
            </Router>
        }

4) Add the required service at Program.cs file.

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddBlocOpenIdConnect();
        
5) Add a configuration file  `wwwroot\api\.configuration\identityserver.json`

       {
         "response_type": "id_token token",
         "scope": "openid profile roles",
         "redirect_uri": "%APPLICATION_URL%",
         "post_logout_redirect_uri": "%APPLICATION_URL%",
         "authority": "%IDENTITY_SERVER_URL%",
         "client_id": "%CLIENT_ID%"
       }
