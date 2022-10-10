namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security.Claims;
    using Blorc.Reflection;

    public static class ObjectExtensions
    {
        public static IEnumerable<Claim> AsClaims(this object instance, string claimType = "")
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(claimType);

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
            ArgumentNullException.ThrowIfNull(instance);

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
                var claimTypeName = claimTypeAttribute?.ClaimType ?? string.Empty;

                foreach (var claim in propertyValue.AsClaims(claimTypeName))
                {
                    yield return claim;
                }
            }
        }

        private static IEnumerable<Claim> EnumClaimsFromPrimitive(this object instance, string claimType)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(claimType);

            if (string.IsNullOrWhiteSpace(claimType))
            {
                yield break;
            }

            yield return new Claim(claimType, instance.ToString() ?? string.Empty);
        }
    }
}



