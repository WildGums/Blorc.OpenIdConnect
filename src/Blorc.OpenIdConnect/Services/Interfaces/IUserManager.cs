namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserManager
    {
        #region Properties
        Dictionary<string, string> Configuration { get; set; }
        #endregion

        #region Methods
        Task SigninRedirectAsync();

        Task SignoutRedirectAsync();

        Task<bool> IsAuthenticatedAsync();

        Task<User> GetUserAsync();
        #endregion
    }
}
