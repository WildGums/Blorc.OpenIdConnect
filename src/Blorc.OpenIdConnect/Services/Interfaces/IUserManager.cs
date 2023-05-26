namespace Blorc.OpenIdConnect
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components.Authorization;

    public interface IUserManager : IDisposable
    {
        event EventHandler<UserActivityEventArgs>? UserActivity;

        event EventHandler<UserInactivityEventArgs>? UserInactivity;

        Task<bool> SignInRedirectAsync(string redirectUri = "");

        Task<bool> SignOutRedirectAsync();

        Task<bool> IsAuthenticatedAsync();
        
        Task<TUser?> GetUserAsync<TUser>(Task<AuthenticationState> authenticationStateTask, JsonSerializerOptions? options = null);

        Task<TUser?> GetUserAsync<TUser>(bool reload = true, JsonSerializerOptions? options = null);
    }
}
