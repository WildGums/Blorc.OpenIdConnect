namespace Blorc.OpenIdConnect.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Blorc.OpenIdConnect.Models;

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
