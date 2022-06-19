namespace Blorc.OpenIdConnect.DemoApp.Models
{
    using System.Text.Json.Serialization;

    public class User : IHasRoles
    {
        [JsonPropertyName("access_token")]
        public string AccessToken
        {
            get;
            set;
        }

        [JsonPropertyName("expires_at")]
        public long ExpiresAt
        {
            get;
            set;
        }

        [JsonPropertyName("profile")]
        public Profile Profile
        {
            get;
            set;
        }

        [JsonPropertyName("session_state")]
        public string SessionState
        {
            get;
            set;
        }

        [JsonPropertyName("token_type")]
        public string TokenType
        {
            get;
            set;
        }

        public IEnumerable<string> Roles
        {
            get
            {
                return Profile?.Roles;
            }
        }
    }
}
