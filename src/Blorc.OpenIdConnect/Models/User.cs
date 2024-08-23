namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class User<TProfile> : IHasRoles 
        where TProfile : Profile
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken
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

        [JsonPropertyName("id_token")]
        public string IdToken
        {
            get;
            set;
        } = string.Empty;

        [JsonPropertyName("profile")]
        public TProfile? Profile
        {
            get;
            set;
        }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken
        {
            get;
            set;
        }

        [JsonPropertyName("scope")]
        public string? Scope
        {
            get;
            set;
        }

        public string[] Scopes
        {
            get
            {
                if (Scope is not null)
                {
                    return Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                }

                return Array.Empty<string>();
            }
        }


        [JsonPropertyName("session_state")]
        public string? SessionState
        {
            get;
            set;
        }

        [JsonPropertyName("token_type")]
        public string? TokenType
        {
            get;
            set;
        }

        [JsonIgnore]
        public IEnumerable<string> Roles
        {
            get
            {
                return Profile?.Roles ?? Array.Empty<string>();
            }
        }
    }
}
