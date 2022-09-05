namespace Blorc.OpenIdConnect
{
    using System.Security.Claims;
    using System.Text.Json.Serialization;

    public class Profile
    {
        [ClaimType(ClaimTypes.Email)]
        [JsonPropertyName("email")]
        public string Email
        {
            get;
            set;
        }

        [JsonPropertyName("email_verified")]
        public bool EmailVerified
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.Surname)]
        [JsonPropertyName("family_name")]
        public string FamilyName
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.GivenName)]
        [JsonPropertyName("given_name")]
        public string GivenName
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.Name)]
        [JsonPropertyName("name")]
        public string Name
        {
            get;
            set;
        }

        [ClaimType("preferred_username")]
        [JsonPropertyName("preferred_username")]
        public string PreferredUsername
        {
            get;
            set;
        }

        [ClaimType(ClaimTypes.Role)]
        [JsonPropertyName("roles")]
        public string[] Roles
        {
            get;
            set;
        }
    }
}
