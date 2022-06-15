namespace Blorc.OpenIdConnect
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomizeHttpRequestMessageDelegatingHandler : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, HttpRequestMessage, Task> _customizeRequestFunction;

        public CustomizeHttpRequestMessageDelegatingHandler(IServiceProvider serviceProvider, Func<IServiceProvider, HttpRequestMessage, Task> customizeRequestFunction)
        {
            _serviceProvider = serviceProvider;
            _customizeRequestFunction = customizeRequestFunction;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_customizeRequestFunction is not null)
            {
                await _customizeRequestFunction(_serviceProvider, request);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
