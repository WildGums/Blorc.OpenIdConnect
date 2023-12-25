namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class OidcProviderOptions
    {
        public OidcProviderOptions()
        {
            // Default values according to docs
            CheckSessionIntervalInSeconds = 2;
            AccessTokenExpiringNotificationTimeInSeconds = 60;
        }

        [JsonPropertyName("authority")]
        public string? Authority { get; set; }

        [JsonPropertyName("automaticSilentRenew")]
        public bool AutomaticSilentRenew { get; set; }

        [JsonPropertyName("timeForUserInactivityAutomaticSignOut")]
        public int TimeForUserInactivityAutomaticSignOut { get; set; }

        [JsonPropertyName("timeForUserInactivityNotification")]
        public int TimeForUserInactivityNotification { get; set; }

        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }

        [JsonPropertyName("post_logout_redirect_uri")]
        public string? PostLogoutRedirectUri { get; set; }

        [JsonPropertyName("redirect_uri")]
        public string? RedirectUri { get; set; }

        [JsonPropertyName("response_type")]
        public string? ResponseType { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("silent_redirect_uri")]
        public string? SilentRedirectUri { get; set; }

        [JsonPropertyName("filterProtocolClaims")]
        public bool FilterProtocolClaims { get; set; }

        [JsonPropertyName("loadUserInfo")]
        public bool LoadUserInfo { get; set; }

        [JsonPropertyName("checkSessionIntervalInSeconds")]
        public int CheckSessionIntervalInSeconds { get; set; }

        [JsonPropertyName("accessTokenExpiringNotificationTimeInSeconds")]
        public int AccessTokenExpiringNotificationTimeInSeconds { get; set; }

        [JsonPropertyName("resource")]
        public string? Resource { get; set; }

        [JsonPropertyName("extraQueryParams")]
        public IDictionary<string, string>? ExtraQueryParams { get; set; }

        [JsonPropertyName("extraTokenParams")]
        public IDictionary<string, string>? ExtraTokenParams { get; set; }
    }
}
