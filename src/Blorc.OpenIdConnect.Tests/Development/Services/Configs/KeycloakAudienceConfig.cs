namespace Blorc.OpenIdConnect.Tests.Development.Services.Configs
{
    using Keycloak.Net.Models.ProtocolMappers;
    using Newtonsoft.Json;

    public class KeycloakAudienceConfig : Config
    {
        [JsonProperty("access.token.claim")]
        public string AddToAccessToken { get; set; }

        [JsonProperty("included.client.audience")]
        public string IncludedClientAudience { get; set; }
    }
}
