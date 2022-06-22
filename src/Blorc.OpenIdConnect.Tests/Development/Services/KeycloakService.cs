namespace Blorc.OpenIdConnect.Tests.Development.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Blorc.OpenIdConnect.Tests.Development.Services.Configs;
    using Flurl.Http;
    using Flurl.Http.Configuration;
    using Keycloak.Net;
    using Keycloak.Net.Models.Clients;
    using Keycloak.Net.Models.ClientScopes;
    using Keycloak.Net.Models.ProtocolMappers;
    using Keycloak.Net.Models.Roles;

    /// <summary>
    /// </summary>
    public class KeycloakService
    {
        private readonly string _password;

        private readonly string _url;

        private readonly string _username;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeycloakService"/> class.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public KeycloakService(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="roleMappingClientName"></param>
        /// <param name="realm"></param>
        /// <param name="ingressUrl"></param>
        /// <param name="pkce"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateClientAsync(string clientName, string roleMappingClientName = "", string realm = "master", string ingressUrl = "", bool pkce = false)
        {
            FlurlHttp.ConfigureClient(_url, c => { c.Settings.HttpClientFactory = new UntrustedCertClientFactory(); });

            var keycloakClient = new KeycloakClient(_url, _username, _password);

            var client = new Client
            {
                Id = clientName,
                Name = clientName,
                DirectAccessGrantsEnabled = !pkce,
                PublicClient = pkce
            };

            if (pkce)
            {
                client.Attributes = new Dictionary<string, object>();
                client.Attributes["pkce.code.challenge.method"] = "S256";
            }

            if (!string.IsNullOrEmpty(ingressUrl))
            {
                client.BaseUrl = ingressUrl;
                client.RedirectUris = new List<string>
                                          {
                                              $"{ingressUrl}/*",
                                          };
                client.WebOrigins = new List<string>
                                        {
                                            ingressUrl.Substring(0, ingressUrl.Length - new Uri(ingressUrl).PathAndQuery.Length),
                                        };

                client.StandardFlowEnabled = true;
                client.ImplicitFlowEnabled = pkce;
            }
            else
            {
                client.StandardFlowEnabled = false;
                client.ImplicitFlowEnabled = pkce;
            }

            if (!string.IsNullOrWhiteSpace(roleMappingClientName))
            {
                client.ProtocolMappers = new List<ClientProtocolMapper>
                                             {
                                                 new()
                                                     {
                                                         Protocol = "openid-connect",
                                                         ProtocolMapper = "oidc-usermodel-client-role-mapper",
                                                         Name = "Client role mapping",
                                                         Config = new KeycloakClientConfig
                                                                      {
                                                                          MultiValued = "true",
                                                                          CustomIdTokenClaim = "true",
                                                                          CustomAccessTokenClaim = "true",
                                                                          CustomUserInfoTokenClaim = "true",
                                                                          UserModelClientRoleMappingClientId = roleMappingClientName,
                                                                          CustomClaimName = "roles",
                                                                          JsonTypeLabel = "String",
                                                                      },
                                                     },
                                             };

            }

            Client storedClient = null;
            try
            {
                storedClient = await keycloakClient.GetClientAsync(realm, client.Id);
            }
            catch (Exception)
            {
            }

            if (storedClient is null)
            {
                await keycloakClient.CreateClientAsync(realm, client);
            }
            else
            {
                storedClient.ProtocolMappers = client.ProtocolMappers;
                storedClient.BaseUrl = client.BaseUrl;
                storedClient.WebOrigins = client.WebOrigins;
                storedClient.RedirectUris = client.RedirectUris;
                storedClient.StandardFlowEnabled = client.StandardFlowEnabled;
                storedClient.DirectAccessGrantsEnabled = client.DirectAccessGrantsEnabled;
                storedClient.ProtocolMappers = client.ProtocolMappers;
                storedClient.PublicClient = client.PublicClient;

                if (pkce)
                {
                    storedClient.Attributes["pkce.code.challenge.method"] = client.Attributes["pkce.code.challenge.method"];
                }

                await keycloakClient.UpdateClientAsync(realm, storedClient.Id, storedClient);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clientScopeName"></param>
        /// <param name="audiences"></param>
        /// <param name="realm"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateClientScopeAsync(string clientScopeName, IEnumerable<string> audiences, string realm = "master")
        {
            FlurlHttp.ConfigureClient(_url, c => { c.Settings.HttpClientFactory = new UntrustedCertClientFactory(); });
            var keycloakClient = new KeycloakClient(_url, _username, _password);

            var clientScope = new ClientScope
            {
                Id = clientScopeName,
                Name = clientScopeName,
                Protocol = "openid-connect",
            };

            var clientScopeProtocolMappers = new List<ProtocolMapper>();

            foreach (var audience in audiences)
            {
                var protocolMapperConfig = new KeycloakAudienceConfig();

                var protocolMapper = new ProtocolMapper
                {
                    Name = $"Audience of {audience}",
                    Protocol = "openid-connect",
                    _ProtocolMapper = "oidc-audience-mapper",
                };

                protocolMapper.Config = protocolMapperConfig;

                protocolMapperConfig.IdTokenClaim = "false";
                protocolMapperConfig.AddToAccessToken = "true";
                protocolMapperConfig.IncludedClientAudience = audience;
                clientScopeProtocolMappers.Add(protocolMapper);
            }

            clientScope.ProtocolMappers = clientScopeProtocolMappers;

            ClientScope storedClientScope = null;
            try
            {
                storedClientScope = await keycloakClient.GetClientScopeAsync(realm, clientScope.Id);
            }
            catch
            {
            }

            if (storedClientScope is not null)
            {
                await keycloakClient.DeleteClientScopeAsync(realm, storedClientScope.Id);
            }

            await keycloakClient.CreateClientScopeAsync(realm, clientScope);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="audiences"></param>
        /// <param name="realm"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateOptionalClientScopeAsync(string clientName, IEnumerable<string> audiences, string realm = "master")
        {
            FlurlHttp.ConfigureClient(_url, c => { c.Settings.HttpClientFactory = new UntrustedCertClientFactory(); });
            var keycloakClient = new KeycloakClient(_url, _username, _password);

            foreach (var audience in audiences)
            {
                await keycloakClient.UpdateOptionalClientScopeAsync(realm, clientName, audience);
            }
        }

        private class UntrustedCertClientFactory : DefaultHttpClientFactory
        {
            public override HttpMessageHandler CreateMessageHandler()
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (a, b, c, d) => true,
                };
            }
        }

        public async Task AddRolesAsync(string clientId, IEnumerable<string> roleNames, string realm = "master")
        {
            FlurlHttp.ConfigureClient(_url, c => { c.Settings.HttpClientFactory = new UntrustedCertClientFactory(); });
            var keycloakClient = new KeycloakClient(_url, _username, _password);

            foreach (var roleName in roleNames)
            {
                Role role = null;
                try
                {
                    role = await keycloakClient.GetRoleByNameAsync(realm, clientId, roleName);
                }
                catch 
                {
                }

                if (role is null)
                {
                    await keycloakClient.CreateRoleAsync(
                        realm,
                        clientId,
                        new Role
                            {
                                Name = roleName
                            });
                }
            }
        }
    }
}
