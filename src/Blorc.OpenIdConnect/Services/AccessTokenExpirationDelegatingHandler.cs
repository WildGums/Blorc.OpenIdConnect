namespace Blorc.OpenIdConnect;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class AccessTokenExpirationDelegatingHandler : DelegatingHandler
{
    private readonly IUserManager _userManager;

    public AccessTokenExpirationDelegatingHandler(IUserManager userManager)
    {
        ArgumentNullException.ThrowIfNull(userManager);

        _userManager = userManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userManager.GetUserAsync<User<Profile>>();
        if (user?.ExpiresAt is not null)
        {
            var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(user.ExpiresAt).UtcDateTime;
            if (DateTime.UtcNow > expiryDateTime)
            {
                await _userManager.SignOutRedirectAsync();
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
