namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class Profile
    {
        [ClaimType("aud")]
        [JsonPropertyName("aud")]
        public JsonElement Aud
        {
            get;
            set;
        } = JsonDocument.Parse("[]").RootElement;

        public string[] Audiences
        {
            get
            {
                if (Aud.ValueKind == JsonValueKind.Array)
                {
                    var result = new List<string>();
                    using var enumerator = Aud.EnumerateArray();

                    foreach (var item in enumerator)
                    {
                        var itemString = item.GetString();
                        if (itemString is not null)
                        {
                            result.Add(itemString);
                        }
                    }

                    return [.. result];
                }
                else if (Aud.ValueKind == JsonValueKind.String)
                {
                    var audienceAsString = Aud.GetString();
                    if (!string.IsNullOrWhiteSpace(audienceAsString))
                    {
                        return [audienceAsString];
                    }
                }

                return [];
            }
        }

        [ClaimType(ClaimTypes.Email)]
        [JsonPropertyName("email")]
        public string? Email
        {
            get;
            set;
        }

        [ClaimType("email_verified")]
        [JsonPropertyName("email_verified")]
        public bool EmailVerified
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.Expiration)]
        [JsonPropertyName("exp")]
        public long Exp
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.Surname)]
        [JsonPropertyName("family_name")]
        public string? FamilyName
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.GivenName)]
        [JsonPropertyName("given_name")]
        public string? GivenName
        {
            get;
            set;
        }

        [ClaimType("iat")]
        [JsonPropertyName("iat")]
        public long Iat
        {
            get;
            set;
        }

        [ClaimType("iss")]
        [JsonPropertyName("iss")]
        public string Iss
        {
            get;
            set;
        } = string.Empty;

        [ClaimType("jti")]
        [JsonPropertyName("jti")]
        public string? Jti
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.Name)]
        [JsonPropertyName("name")]
        public string? Name
        {
            get;
            set;
        }

        [ClaimType("nickname")]
        [JsonPropertyName("nickname")]
        public string? Nickname
        {
            get;
            set;
        }

        [ClaimType("nonce")]
        [JsonPropertyName("nonce")]
        public string? Nonce
        {
            get;
            set;
        }

        [ClaimType("phone_number")]
        [JsonPropertyName("phone_number")]
        public string? PhoneNumber
        {
            get;
            set;
        }

        [ClaimType("phone_number_verified")]
        [JsonPropertyName("phone_number_verified")]
        public bool PhoneNumberVerified
        {
            get;
            set;
        }

        [ClaimType("picture")]
        [JsonPropertyName("picture")]
        public string? Picture
        {
            get;
            set;
        }

        [ClaimType("preferred_username")]
        [JsonPropertyName("preferred_username")]
        public string? PreferredUsername
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.Role)]
        [JsonPropertyName("roles")]
        public string[]? Roles
        {
            get;
            set;
        }

        [ClaimType("sub")]
        [JsonPropertyName("sub")]
        public string Sub
        {
            get;
            set;
        } = string.Empty;

        [ClaimType("username")]
        [JsonPropertyName("username")]
        public string? Username
        {
            get;
            set;
        }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData
        {
            get;
            set;
        }
    }
}
