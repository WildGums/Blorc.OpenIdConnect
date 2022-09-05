namespace Blorc.OpenIdConnect.Tests.Development
{
    using System.Threading.Tasks;
    using Blorc.OpenIdConnect.Tests.Development.Services;
    using NUnit.Framework;

    [TestFixture]
    public class Environment
    {
        [Test]
        [Explicit]
        public async Task InitializeKeycloakAsync()
        {
            var keycloakService = new KeycloakService("http://localhost:5002/", "admin", "Password123!");

            await keycloakService.CreateClientAsync(Clients.DemoApp, Clients.DemoApp, ingressUrl: "http://localhost:5001/", pkce: true);

            var audiences = Scopes.From(Audiences.DemoApi);
            foreach (var audience in audiences)
            {
                await keycloakService.CreateClientAsync(audience, Clients.DemoApp);
                await keycloakService.CreateClientScopeAsync(audience, Scopes.From(audience));
            }

            await keycloakService.UpdateOptionalClientScopeAsync(Clients.DemoApp, audiences);
        }
    }
}
