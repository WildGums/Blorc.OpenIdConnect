namespace Blorc.OpenIdConnect.Tests.Services
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AccessTokenDelegatingHandlerFact
    {
        [TestFixture]
        public class The_SendAsync_Method
        {
            [Test]
            public async Task Sets_The_Access_Token_As_Authorization_Header_Async()
            {
                // Arrange
                var userManagerMock = new Mock<IUserManager>();
                var user = new User<Profile>
                {
                    AccessToken = Guid.NewGuid().ToString(),
                };

                userManagerMock.Setup(userManager => userManager.GetUserAsync<User<Profile>>(It.IsAny<bool>(), It.IsAny<JsonSerializerOptions>())).ReturnsAsync(user);

                using var handler = new TestableAccessTokenDelegatingHandler(userManagerMock.Object);
                handler.InnerHandler = new HttpClientHandler();

                using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

                // Act
                await handler.SendAsync(request, CancellationToken.None);

                // Assert

                Assert.That(request.Headers.Authorization, Is.Not.Null);
                Assert.That(request.Headers.Authorization.Scheme, Is.EqualTo("Bearer"));
                Assert.That(request.Headers.Authorization.Parameter, Is.EqualTo(user.AccessToken));
            }

            private class TestableAccessTokenDelegatingHandler : AccessTokenDelegatingHandler
            {
                public TestableAccessTokenDelegatingHandler(IUserManager userManager)
                    : base(userManager) { }

                public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    return base.SendAsync(request, cancellationToken);
                }
            }
        }
    }
}
