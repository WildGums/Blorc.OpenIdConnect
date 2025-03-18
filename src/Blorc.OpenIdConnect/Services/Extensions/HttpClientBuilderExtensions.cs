namespace Blorc.OpenIdConnect
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddAccessToken(this IHttpClientBuilder @this)
        {
            ArgumentNullException.ThrowIfNull(@this);

            return @this.AddHttpMessageHandler<AccessTokenDelegatingHandler>();
        }

        public static IHttpClientBuilder AddAccessTokenExpiration(this IHttpClientBuilder @this)
        {
            ArgumentNullException.ThrowIfNull(@this);

            return @this.AddHttpMessageHandler<AccessTokenExpirationDelegatingHandler>();
        }

        public static IHttpClientBuilder CustomizeHttpRequestMessage(this IHttpClientBuilder @this, Func<IServiceProvider, HttpRequestMessage, Task> customizationRequest)
        {
            ArgumentNullException.ThrowIfNull(@this);
            ArgumentNullException.ThrowIfNull(customizationRequest);

            return @this.AddHttpMessageHandler(provider => ActivatorUtilities.CreateInstance<CustomizeHttpRequestMessageDelegatingHandler>(provider, customizationRequest));
        }
    }
}
