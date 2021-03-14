namespace Blorc.OpenIdConnect
{
    using System.Linq;
    using System.Text.Json;

    public class User
    {
        private readonly JsonElement _jsonElement;

        private Profile _profile;

        public User(JsonElement jsonElement)
        {
            _jsonElement = jsonElement;
        }

        public string AccessToken
        {
            get
            {
                if (_jsonElement.TryGetProperty("access_token", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public long ExpiresAt
        {
            get
            {
                if (_jsonElement.TryGetProperty("expires_at", out var value))
                {
                    return value.GetInt64();
                }

                return 0;
            }
        }

        public JsonElement JsonElement => _jsonElement;

        public Profile Profile
        {
            get
            {
                if (_profile is null)
                {
                    if (_jsonElement.TryGetProperty("profile", out var value))
                    {
                        _profile = new Profile(value);
                    }
                }

                return _profile;
            }
        }

        public string SessionState
        {
            get
            {
                if (_jsonElement.TryGetProperty("session_state", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public string TokenType
        {
            get
            {
                if (_jsonElement.TryGetProperty("access_token", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public bool IsInRole(string role)
        {
            if (Profile?.Roles == null)
            {
                return false;
            }

            return Profile.Roles.Contains(role);
        }
    }
}
