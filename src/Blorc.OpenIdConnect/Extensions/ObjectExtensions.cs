namespace Blorc.OpenIdConnect
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security.Claims;
    using Blorc.Reflection;

    public static class ObjectExtensions
    {
        public static IEnumerable<Claim> AsClaims(this object instance, string claimType = null)
        {
            if (instance.GetType().IsCollection() && instance is IEnumerable items)
            {
                return items.AsClaims(claimType);
            }

            if (instance.GetType().IsPrimitiveEx())
            {
                return instance.EnumClaimsFromPrimitive(claimType);
            }

            return instance.EnumClaimsFromObjectProperties();
        }

        private static IEnumerable<Claim> EnumClaimsFromObjectProperties(this object instance)
        {
            var propertyInfos = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in propertyInfos)
            {
                if (!(propertyInfo.GetMethod?.IsPublic ?? false))
                {
                    continue;
                }

                var propertyValue = propertyInfo.GetValue(instance);
                if (propertyValue is null)
                {
                    continue;
                }

                var claimTypeAttribute = propertyInfo.GetCustomAttribute<ClaimTypeAttribute>();
                foreach (var claim in propertyValue.AsClaims(claimTypeAttribute?.ClaimType ?? string.Empty))
                {
                    yield return claim;
                }
            }
        }

        private static IEnumerable<Claim> EnumClaimsFromPrimitive(this object instance, string claimType)
        {
            if (string.IsNullOrWhiteSpace(claimType))
            {
                yield break;
            }

            yield return new Claim(claimType, instance?.ToString() ?? string.Empty);
        }
    }
}



