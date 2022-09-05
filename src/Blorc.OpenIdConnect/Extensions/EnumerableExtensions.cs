namespace Blorc.OpenIdConnect
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    public static class EnumerableExtensions
    {
        public static IEnumerable<Claim> AsClaims(this IEnumerable items, string claimType)
        {
            return items.OfType<object>().SelectMany(item => item.AsClaims(claimType));
        }
    }
}
