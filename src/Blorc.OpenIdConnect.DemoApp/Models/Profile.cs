namespace Blorc.OpenIdConnect.DemoApp
{
    using System.Text.Json.Serialization;

    public class Profile
    {
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

        [JsonPropertyName("family_name")]
        public string FamilyName
        {
            get;
            set;
        }

        [JsonPropertyName("given_name")]
        public string GivenName
        {
            get;
            set;
        }

        [JsonPropertyName("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonPropertyName("preferred_username")]
        public string PreferredUsername
        {
            get;
            set;
        }

        [JsonPropertyName("roles")]
        public string[] Roles
        {
            get;
            set;
        }
    }
}
