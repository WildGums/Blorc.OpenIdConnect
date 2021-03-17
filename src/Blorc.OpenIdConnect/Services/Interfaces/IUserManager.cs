namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components.Authorization;

    public interface IUserManager
    {
        Dictionary<string, string> Configuration { get; }

        Task SigninRedirectAsync();

        Task SignoutRedirectAsync();

        Task<bool> IsAuthenticatedAsync();

        Task<User> GetUserAsync(bool reload = true);

        Task<User> GetUserAsync(Task<AuthenticationState> authenticationStateTask);
    }
}
