namespace Blorc.OpenIdConnect
{
    using System.Linq;

    public static class HasRolesExtensions
    {
        public static bool IsInRole(this IHasRoles hasRoles, string role)
        {
            if (hasRoles.Roles is null)
            {
                return false;
            }

            return hasRoles.Roles.Contains(role);
        }
    }
}
