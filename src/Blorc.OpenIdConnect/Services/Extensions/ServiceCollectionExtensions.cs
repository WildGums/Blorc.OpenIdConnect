namespace Blorc.OpenIdConnect
{
    using System;

    using Blorc.OpenIdConnect.Models;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        #region Methods
        public static void AddBlocOpenIdConnect(this IServiceCollection services, Action<OidcProviderOptions> configure = null)
        {
            services.AddSingleton<IUserManager, UserManager>();
            services.AddTransient<IHttpClient, TokenBasedSecuredHttpClient>();
            services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, OpenIdConnectAuthenticationStateProvider>();
            if (configure is not null)
            {
                var options = new OidcProviderOptions();
                configure(options);
                services.AddSingleton(options);
            }
            
        }
        #endregion
    }
}
