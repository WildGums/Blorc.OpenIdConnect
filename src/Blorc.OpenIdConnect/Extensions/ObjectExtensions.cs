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
            else  if (instanceType.IsCollection() && instance is IEnumerable items)
            {
                foreach (var item in items)
                {
                    foreach (var claim in item.AsClaims())
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
                    var propertyValue = propertyInfo.GetValue(instance);
                    if (propertyValue is null)
                    {
                        continue;
                    }

                    var propertyValueType = propertyValue.GetType();
                    var claimTypeAttribute = propertyInfo.GetCustomAttribute<ClaimTypeAttribute>();
                    if (claimTypeAttribute is not null)
                    {
                        if (propertyValueType.IsPrimitiveEx())
                        {
                            yield return new Claim(claimTypeAttribute.ClaimType, propertyValue?.ToString() ?? string.Empty);
                        }
                        else if (propertyValueType.IsCollection() && propertyValue is IEnumerable valueItems)
                        {
                            foreach (var item in valueItems)
                            {
                                foreach (var claim in item.AsClaims(claimTypeAttribute.ClaimType))
                                {
                                    yield return claim;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var claim in propertyValue.AsClaims())
                        {
                            yield return claim;
                        }
                    }
                }
            }
        }
    }
}
