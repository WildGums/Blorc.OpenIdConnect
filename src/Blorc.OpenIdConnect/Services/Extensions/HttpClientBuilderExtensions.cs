namespace Blorc.OpenIdConnect
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public static class HttpClientBuilderExtensions
    {
        public static void AddAccessToken(this IHttpClientBuilder @this)
        {
            @this.AddHttpMessageHandler<AccessTokenDelegatingHandler>();
        }

        public static void CustomizeHttpRequestMessage(this IHttpClientBuilder @this, Func<IServiceProvider, HttpRequestMessage, Task> customizationRequest)
        {
            @this.AddHttpMessageHandler(provider => ActivatorUtilities.CreateInstance<CustomizeHttpRequestMessageDelegatingHandler>(provider, customizationRequest));
        }
    }
}
