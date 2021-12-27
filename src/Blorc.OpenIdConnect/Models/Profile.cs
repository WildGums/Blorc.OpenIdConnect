namespace Blorc.OpenIdConnect
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    public class Profile : IProfile
    {
        private readonly JsonElement _jsonElement;

        private List<string> _roles;

        public Profile(JsonElement jsonElement)
        {
            _jsonElement = jsonElement;
        }

        public string Email
        {
            get
            {
                if (_jsonElement.TryGetProperty("email", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public bool EmailVerified
        {
            get
            {
                if (_jsonElement.TryGetProperty("email_verified", out var value))
                {
                    return value.GetBoolean();
                }

                return false;
            }
        }

        public string FamilyName
        {
            get
            {
                if (_jsonElement.TryGetProperty("family_name", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public string GivenName
        {
            get
            {
                if (_jsonElement.TryGetProperty("given_name", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public JsonElement JsonElement => _jsonElement;

        public string Name
        {
            get
            {
                if (_jsonElement.TryGetProperty("name", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public string PreferredUsername
        {
            get
            {
                if (_jsonElement.TryGetProperty("preferred_username", out var value))
                {
                    return value.GetString();
                }

                return string.Empty;
            }
        }

        public string[] Roles
        {
            get
            {
                if (_roles is null)
                {
                    if (_jsonElement.TryGetProperty("roles", out var value))
                    {
                        _roles = new List<string>();
                        using (var elements = value.EnumerateArray())
                        {
                            foreach (var element in elements)
                            {
                                _roles.Add(element.GetString());
                            }
                        }
                    }
                }

                return _roles is null ? Array.Empty<string>() : _roles.ToArray();
            }
        }
    }
}
