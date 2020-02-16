# Blorc.OpenIdConnect

## Usage

1) Install Blorc.OpenIdConnect via NuGet.

2) Update App.razor content like this

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

3) Update Program.cs

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddBlocOpenIdConnect();