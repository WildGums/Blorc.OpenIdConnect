namespace Blorc.OpenIdConnect.Tests.Development.Services.Configs
{
    using Keycloak.Net.Models.Clients;
    using Newtonsoft.Json;

    public class KeycloakClientConfig : ClientConfig
    {
        [JsonProperty("multivalued")]
        public string MultiValued { get; set; }

        [JsonProperty("usermodel.clientRoleMapping.clientId")]
        public string UserModelClientRoleMappingClientId { get; set; }

        [JsonProperty("userinfo.token.claim")]
        public string CustomUserInfoTokenClaim { get; set; }

        [JsonProperty("claim.name")]
        public string CustomClaimName { get; set; }

        [JsonProperty("access.token.claim")]
        public string CustomAccessTokenClaim { get; set; }

        [JsonProperty("id.token.claim")]
        public string CustomIdTokenClaim { get; set; }

        [JsonProperty("jsonType.label")]
        public string JsonTypeLabel { get; set; }
    }
}
