namespace Blorc.OpenIdConnect.Services.Extensions
{
    using Blorc.OpenIdConnect.Services;
    using Blorc.OpenIdConnect.Services.Interfaces;

    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        #region Methods
        public static void AddBlocOpenIdConnect(this IServiceCollection services)
        {
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IHttpClient, TokenBasedSecuredHttpClient>();
            services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, OpenIdConnectAuthenticationStateProvider>();
        }
        #endregion
    }
}
