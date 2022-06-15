namespace Blorc.OpenIdConnect
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class AccessTokenDelegatingHandler : DelegatingHandler
    {
        private readonly IUserManager _userManager;

        public AccessTokenDelegatingHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync();
            if (user is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
