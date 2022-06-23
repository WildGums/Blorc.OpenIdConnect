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
            if (instance is null)
            {
                yield break;
            }

            var instanceType = instance.GetType();
            if (instanceType.IsPrimitiveEx())
            {
                if (string.IsNullOrWhiteSpace(claimType))
                {
                    yield break;
                }

                yield return new Claim(claimType, instance?.ToString() ?? string.Empty);
            }
            else if (instanceType.IsCollection() && instance is IEnumerable items)
            {
                foreach (var item in items)
                {
                    foreach (var claim in item.AsClaims(claimType))
                    {
                        yield return claim;
                    }
                }
            }
            else
            {
                var propertyInfos = instanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
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
        }
    }
}
