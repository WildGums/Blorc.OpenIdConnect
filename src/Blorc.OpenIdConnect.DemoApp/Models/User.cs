namespace Blorc.OpenIdConnect.DemoApp
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class User<TProfile> : IHasRoles where TProfile : Profile
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
        public TProfile Profile
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

        [JsonIgnore]
        public IEnumerable<string> Roles
        {
            get
            {
                return Profile?.Roles;
            }
        }
    }
}
