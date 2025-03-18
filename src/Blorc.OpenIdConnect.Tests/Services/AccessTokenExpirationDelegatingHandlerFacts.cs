namespace Blorc.OpenIdConnect.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Time.Testing;
    using Moq;
    using Moq.Protected;
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
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

                using var httpResponseMessage = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Fake response")
                };

                mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(httpResponseMessage);

                var userManagerMock = new Mock<IUserManager>();
                var now = DateTimeOffset.UtcNow;
                var user = new User<Profile>
                {
                    ExpiresAt = now.AddSeconds(-10).ToUnixTimeSeconds()  // Expired token
                };

                userManagerMock.Setup(userManager => userManager.GetUserAsync<User<Profile>>(It.IsAny<bool>(), It.IsAny<JsonSerializerOptions>())).ReturnsAsync(user);

                var fakeTimeProvider = new FakeTimeProvider();
                fakeTimeProvider.SetUtcNow(now);

                using var handler = new TestableAccessTokenExpirationDelegatingHandler(userManagerMock.Object, fakeTimeProvider);
                handler.InnerHandler = mockHttpMessageHandler.Object;

                using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

                // Act
                await handler.SendAsync(request, CancellationToken.None);

                // Assert
                userManagerMock.Verify(um => um.SignOutRedirectAsync(), Times.Once);
            }

            private class TestableAccessTokenExpirationDelegatingHandler : AccessTokenExpirationDelegatingHandler
            {
                public TestableAccessTokenExpirationDelegatingHandler(IUserManager userManager, TimeProvider timeProvider)
                    : base(userManager, timeProvider) { }

                public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    return base.SendAsync(request, cancellationToken);
                }
            }
        }
    }
}
