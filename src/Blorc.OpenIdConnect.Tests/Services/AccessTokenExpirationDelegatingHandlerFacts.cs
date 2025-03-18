namespace Blorc.OpenIdConnect.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AccessTokenExpirationDelegatingHandlerFacts
    {
        [TestFixture]
        public class The_SendAsync_Method
        {
            [Test]
            public async Task Calls_SignOutRedirectAsync_When_The_Token_Is_Expired_Async()
            {
                // Arrange
                var userManagerMock = new Mock<IUserManager>();
                var user = new User<Profile>
                {
                    ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(-10).ToUnixTimeSeconds()  // Expired token
                };

                userManagerMock.Setup(userManager => userManager.GetUserAsync<User<Profile>>(It.IsAny<bool>(), It.IsAny<JsonSerializerOptions>())).ReturnsAsync(user);

                using var handler = new TestableAccessTokenExpirationDelegatingHandler(userManagerMock.Object);
                handler.InnerHandler = new HttpClientHandler();

                using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

                // Act
                await handler.SendAsync(request, CancellationToken.None);

                // Assert
                userManagerMock.Verify(um => um.SignOutRedirectAsync(), Times.Once);
            }

            private class TestableAccessTokenExpirationDelegatingHandler : AccessTokenExpirationDelegatingHandler
            {
                public TestableAccessTokenExpirationDelegatingHandler(IUserManager userManager)
                    : base(userManager) { }

                public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    return base.SendAsync(request, cancellationToken);
                }
            }
        }
    }
}
