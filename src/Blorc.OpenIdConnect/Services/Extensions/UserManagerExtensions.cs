namespace Blorc.OpenIdConnect
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components.Authorization;

    public static class UserManagerExtensions
    {
        public static Task<User<Profile>?> GetUserAsync(this IUserManager userManager)
        {
            ArgumentNullException.ThrowIfNull(userManager);

            return userManager.GetUserAsync<User<Profile>>();
        }

        public static Task<User<Profile>?> GetUserAsync(this IUserManager userManager, Task<AuthenticationState> authenticationStateTask, JsonSerializerOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(authenticationStateTask);

            return userManager.GetUserAsync<User<Profile>>(authenticationStateTask, options);
        }
    }
}
