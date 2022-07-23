namespace Blorc.OpenIdConnect
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components.Authorization;

    public interface IUserManager
    {
        Task SigninRedirectAsync();

        Task SignoutRedirectAsync();

        Task<bool> IsAuthenticatedAsync();
        
        Task<TUser> GetUserAsync<TUser>(Task<AuthenticationState> authenticationStateTask, JsonSerializerOptions options = null);

        Task<TUser> GetUserAsync<TUser>(bool reload = true, JsonSerializerOptions options = null);

        event EventHandler<UserInactivityEventArgs> UserInactivity;

        event EventHandler UserActivity;
    }
}
