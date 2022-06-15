namespace Blorc.OpenIdConnect
{
    using System;

    using Blorc.OpenIdConnect.Models;

    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddBlocOpenIdConnect(this IServiceCollection services, Action<OidcProviderOptions> configure = null)
        {
            services.AddTransient<IHttpClient, TokenBasedSecuredHttpClient>();
            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton<AuthenticationStateProvider, OpenIdConnectAuthenticationStateProvider>();
            services.AddScoped<AccessTokenDelegatingHandler>();

            if (configure is not null)
            {
                var options = new OidcProviderOptions();
                configure(options);
                services.AddSingleton(options);
            }
        }
    }
}
