namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components.Authorization;

    public class OpenIdConnectAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IUserManager _userManager;

        public OpenIdConnectAuthenticationStateProvider(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(principal);
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Profile.Name), new Claim(ClaimTypes.Email, user.Profile.Email) };
            foreach (var profileRole in user.Profile.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, profileRole));
            }

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Blorc.OpenIdConnect"));

            return new AuthenticationState(claimsPrincipal);
        }
    }
}
