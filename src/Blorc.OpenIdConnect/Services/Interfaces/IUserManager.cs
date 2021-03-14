namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    public interface IUserManager
    {
        Dictionary<string, string> Configuration { get; set; }

        event EventHandler AccessTokenExpiring;

        Task SigninRedirectAsync();

        Task SignoutRedirectAsync();

        Task<bool> IsAuthenticatedAsync();

        Task<User> GetUserAsync();

        Task<JsonElement?> GetUserJsonElementAsync();
    }
}
