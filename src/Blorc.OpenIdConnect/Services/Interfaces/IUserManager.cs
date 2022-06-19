namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components.Authorization;

    public interface IUserManager
    {
        Dictionary<string, object> Configuration { get; }

        Task SigninRedirectAsync();

        Task SignoutRedirectAsync();

        Task<bool> IsAuthenticatedAsync();

        Task<IUser> GetUserAsync(bool reload = true);

        Task<IUser> GetUserAsync(Task<AuthenticationState> authenticationStateTask);
        
        Task<TUser> GetUserAsync<TUser>(Task<AuthenticationState> authenticationStateTask, JsonSerializerOptions options = null);

        Task<TUser> GetUserAsync<TUser>(bool reload = true, JsonSerializerOptions options = null);
    }
}
