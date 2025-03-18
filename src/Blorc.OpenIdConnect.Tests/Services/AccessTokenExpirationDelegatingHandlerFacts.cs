namespace Blorc.OpenIdConnect.Tests.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
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

                using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

                // Act
                using var response = await handler.SendAsync(request, CancellationToken.None);

                // Assert
                userManagerMock.Verify(um => um.SignOutRedirectAsync(), Times.Once);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(response.ReasonPhrase, Is.EqualTo("Access token expired"));
            }

            [Test]
            public async Task Does_Not_Call_SignOutRedirectAsync_When_The_Token_Is_Not_Expired_Async()
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
                    ExpiresAt = now.AddSeconds(10).ToUnixTimeSeconds() 
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
                userManagerMock.Verify(um => um.SignOutRedirectAsync(), Times.Never);
            }

            private sealed class TestableAccessTokenExpirationDelegatingHandler : AccessTokenExpirationDelegatingHandler
            {
                public TestableAccessTokenExpirationDelegatingHandler(IUserManager userManager, TimeProvider timeProvider)
                    : base(userManager, timeProvider) { }

                // NOTE: The 'new' keyword is used here to define the 'SendAsync'
                // method with the same name as the protected method in the base class,
                // but to make it publicly accessible within this testable implementation.
                public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    return base.SendAsync(request, cancellationToken);
                }
            }
        }
    }
}
