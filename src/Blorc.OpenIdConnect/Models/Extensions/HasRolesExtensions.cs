namespace Blorc.OpenIdConnect
{
    using System;
    using System.Linq;

    public static class HasRolesExtensions
    {
        public static bool IsInRole(this IHasRoles hasRoles, string role)
        {
            ArgumentNullException.ThrowIfNull(hasRoles);
            ArgumentNullException.ThrowIfNull(role);

            return hasRoles.Roles.Contains(role);
        }
    }
}
