namespace Blorc.OpenIdConnect
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class AccessTokenDelegatingHandler : DelegatingHandler
    {
        private readonly IUserManager _userManager;

        public AccessTokenDelegatingHandler(IUserManager userManager)
        {
            ArgumentNullException.ThrowIfNull(userManager);

            _userManager = userManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await _userManager.GetUserAsync<User<Profile>>();
            if (user is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
