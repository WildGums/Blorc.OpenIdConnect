namespace Blorc.OpenIdConnect.Models
{
    using System.Text.Json.Serialization;

    public class OidcProviderOptions
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        
        [JsonPropertyName("authority")]
        public string Authority { get; set; }
        
        [JsonPropertyName("post_logout_redirect_uri")]
        public string PostLogoutRedirectUri { get; set; }  
        
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }        
        
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }
    }
}
