namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserManager
    {
        Dictionary<string, string> Configuration { get; set; }

        Task SigninRedirectAsync();

        Task SignoutRedirectAsync();

        Task<bool> IsAuthenticatedAsync();

        Task<User> GetUserAsync();
    }
}
