# Blorc.OpenIdConnect

The right way to use OpenIdConnect on Blazor.

## Usage

1) Install Blorc.OpenIdConnect via NuGet.

2) Include `Blorc.Core/injector.js` the index.html file:

        <head>
                ...
                <script src="_content/Blorc.Core/injector.js"></script>
                ...
        </head>        
 
3) Update App.razor content like this:

	@using Microsoft.AspNetCore.Components.Authorization
	
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


4) Add the required service and update Program.cs file as follow:

 	builder.Services.AddBlorcCore();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddBlocOpenIdConnect(
			options =>
			{
				builder.Configuration.Bind("IdentityServer", options);
		                options.RedirectUri = builder.HostEnvironment.BaseAddress;
		                options.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
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
        
5) Add a configuration file `wwwroot\appsetings.json`

	{
	  "IdentityServer": {
	    "ResponseType": "id_token token",
	    "Scope": "openid profile %API-NAME%",
	    "RedirectUri": "%APPLICATION_URL%",
	    "PostLogoutRedirectUri": "%APPLICATION_URL%",
	    "Authority": "%IDENTITY_SERVER_URL%",
	    "ClientId": "%CLIENT_ID%"
	  }
	}