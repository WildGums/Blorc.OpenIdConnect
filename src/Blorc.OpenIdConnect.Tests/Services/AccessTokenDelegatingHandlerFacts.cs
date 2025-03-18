namespace Blorc.OpenIdConnect.Tests.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;
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
                var user = new User<Profile>
                {
                    AccessToken = Guid.NewGuid().ToString(),
                };

                userManagerMock.Setup(userManager => userManager.GetUserAsync<User<Profile>>(It.IsAny<bool>(), It.IsAny<JsonSerializerOptions>())).ReturnsAsync(user);

                using var handler = new TestableAccessTokenDelegatingHandler(userManagerMock.Object);
                handler.InnerHandler = mockHttpMessageHandler.Object;

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
