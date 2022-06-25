namespace Blorc.OpenIdConnect
{
    using System.Threading.Tasks;

    public static class UserManagerExtensions
    {
        public static Task<User<Profile>> GetUserAsync(this IUserManager userManager)
        {
            return userManager.GetUserAsync<User<Profile>>();
        }
    }
}
