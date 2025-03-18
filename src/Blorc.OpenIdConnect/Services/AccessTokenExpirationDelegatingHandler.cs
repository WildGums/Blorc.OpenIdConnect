namespace Blorc.OpenIdConnect;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class AccessTokenExpirationDelegatingHandler : DelegatingHandler
{
    private readonly IUserManager _userManager;

    private readonly TimeProvider _timeProvider;

    public AccessTokenExpirationDelegatingHandler(IUserManager userManager, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userManager.GetUserAsync<User<Profile>>();
        if (user?.ExpiresAt is not null)
        {
            var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(user.ExpiresAt).UtcDateTime;
            if (_timeProvider.GetUtcNow() > expiryDateTime)
            {
                await _userManager.SignOutRedirectAsync();
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
