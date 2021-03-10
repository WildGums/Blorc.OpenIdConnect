namespace Blorc.OpenIdConnect
{
    using System;
    using System.Text.Json.Serialization;

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

    public class OidcProviderOptions
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        
        [JsonPropertyName("authority")]
        public string Authority { get; set; }
        
        [JsonPropertyName("post_logout_redirect_uri")]
        public string PostLogoutRedirectUri { get; set; }  
        
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }        
        
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }
    }
}
