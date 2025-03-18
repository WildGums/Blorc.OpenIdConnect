namespace Blorc.OpenIdConnect
{
    using System;
    using Blorc.OpenIdConnect;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddBlorcOpenIdConnect(this IServiceCollection services, Action<OidcProviderOptions>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton<AuthenticationStateProvider, OpenIdConnectAuthenticationStateProvider>();
            services.AddScoped<AccessTokenDelegatingHandler>();
            services.AddScoped<AccessTokenExpirationDelegatingHandler>();

            if (configure is not null)
            {
                var options = new OidcProviderOptions();

                configure(options);

                if (options.ResponseType != "code")
                {
                    throw new NotSupportedException("Only the authorization code flow with PKCE is supported. The expected value for ResponseType is 'code'.");
                }

                services.AddSingleton(options);
            }
        }
    }
}
