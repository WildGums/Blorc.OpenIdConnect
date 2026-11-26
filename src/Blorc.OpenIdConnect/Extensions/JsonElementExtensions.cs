namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text.Json;

    public static partial class JsonElementExtensions
    {
        public static IEnumerable<Claim> AsClaims(this JsonElement element, string claimType = "")
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        using var enumerator = element.EnumerateObject();
                        foreach (JsonProperty property in enumerator)
                        {
                            // For objects, recursively process each property.
                            // Use the property name as part of the claim type.
                            string nestedClaimType = string.IsNullOrEmpty(claimType) ? property.Name : $"{claimType}.{property.Name}";
                            foreach (var claim in AsClaims(property.Value, nestedClaimType))
                            {
                                yield return claim;
                            }
                        }
                    }
                    break;

                case JsonValueKind.Array:
                    {
                        using var enumerator = element.EnumerateArray();
                        int index = 0;
                        foreach (JsonElement item in enumerator)
                        {
                            // For arrays, process each item and use the index in the claim type.
                            string indexedClaimType = $"{claimType}[{index}]";
                            foreach (var claim in AsClaims(item, indexedClaimType))
                            {
                                yield return claim;
                            }
                            index++;
                        }
                    }
                    break;

                case JsonValueKind.String:
                    yield return new Claim(claimType, element.GetString()!);
                    break;

                case JsonValueKind.Number:
                    yield return new Claim(claimType, element.GetRawText());
                    break;

                case JsonValueKind.True:
                    yield return new Claim(claimType, bool.TrueString);
                    break;

                case JsonValueKind.False:
                    yield return new Claim(claimType, bool.FalseString);
                    break;

                case JsonValueKind.Null:
                    // Ignore null values.
                    break;

                default:
                    throw new InvalidOperationException($"Unknown JSON token: {element.ValueKind}");
            }
        }
    }
}
