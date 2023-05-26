namespace Blorc.OpenIdConnect.Tests.Services
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Blorc.OpenIdConnect;
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.JSInterop;
    using Moq;
    using NUnit.Framework;
    using Is = NUnit.DeepObjectCompare.Is;

    [TestFixture]
    public class UserManagerFacts
    {
        [TestFixture]
        public class The_GetUserAsync_Generic_Method
        {
            [Test]
            public async Task Returns_An_Instance_Of_The_Specified_User_Type_Async()
            {
                var jsRuntimeMock = new Mock<IJSRuntime>();
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.IsInitialized", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.IsAuthenticated", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Navigation.IsRedirected", It.IsAny<object?[]>()))
                    .ReturnsAsync(false);

                var jsonElement = JsonSerializer.Deserialize<JsonElement>("{\r\n  \"access_token\": \"1234567890\"\r\n}");

                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.GetUser", It.IsAny<object?[]>()))
                    .ReturnsAsync(jsonElement);

                var oidcProviderOptions = new OidcProviderOptions();

                var navigationManagerStub = new Stub<NavigationManager>(new NavigationManagerStub());
                navigationManagerStub.SetField("_isInitialized", true);
                navigationManagerStub.SetField("_uri", "http://localhost");

                using var userManager = new UserManager(new NullLogger<UserManager>(), jsRuntimeMock.Object, navigationManagerStub.Instance, oidcProviderOptions);

                var user = await userManager.GetUserAsync<User<Profile>>();

                Assert.IsNotNull(user);
            }

            [Test]
            public async Task Returns_An_Instance_Of_The_Specified_User_Type_With_The_Expected_Values_In_All_Properties_Async()
            {
                var jsRuntimeMock = new Mock<IJSRuntime>();
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.IsInitialized", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.IsAuthenticated", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Navigation.IsRedirected", It.IsAny<object?[]>()))
                    .ReturnsAsync(false);

                var expectedUser = new User<Profile>
                {
                    AccessToken = "1234567890",
                    Profile = new Profile
                                {
                                    Roles = new[] { "Administrator", "System Administrator" },
                                    Email = "jane.doe@blorc.com",
                                    EmailVerified = true,
                                    FamilyName = "Doe",
                                    GivenName = "Jane",
                                    Name = "Jane Doe",
                                    PreferredUsername = "jane.doe"
                                },
                    ExpiresAt = 10,
                    SessionState = "alskjdhflaskjdhflaksjdhqwpoyir",
                    TokenType = "Bearer"
                };

                var serializedExpectedUser = JsonSerializer.Serialize(expectedUser);

                var jsonElement = JsonSerializer.Deserialize<JsonElement>(serializedExpectedUser);

                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.GetUser", It.IsAny<object?[]>()))
                    .ReturnsAsync(jsonElement);

                var oidcProviderOptions = new OidcProviderOptions();

                var navigationManagerStub = new Stub<NavigationManager>(new NavigationManagerStub());
                navigationManagerStub.SetField("_isInitialized", true);
                navigationManagerStub.SetField("_uri", "http://localhost");

                using var userManager = new UserManager(new NullLogger<UserManager>(), jsRuntimeMock.Object, navigationManagerStub.Instance, oidcProviderOptions);

                var user = await userManager.GetUserAsync<User<Profile>>();

                Assert.That(user, Is.DeepEqualTo(expectedUser));
            }

            [Test]
            public async Task Calls_NavigationManager_With_An_Url_Without_Token_Information_When_Is_Redirected_Async()
            {
                var jsRuntimeMock = new Mock<IJSRuntime>();
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.IsInitialized", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.IsAuthenticated", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);

                var expectedUser = new User<Profile>
                {
                    AccessToken = "1234567890",
                    Profile = new Profile
                                {
                                    Roles = new[] { "Administrator", "System Administrator" },
                                    Email = "jane.doe@blorc.com",
                                    EmailVerified = true,
                                    FamilyName = "Doe",
                                    GivenName = "Jane",
                                    Name = "Jane Doe",
                                    PreferredUsername = "jane.doe"
                                },
                    ExpiresAt = 10,
                    SessionState = "alskjdhflaskjdhflaksjdhqwpoyir",
                    TokenType = "Bearer"
                };

                var serializedExpectedUser = JsonSerializer.Serialize(expectedUser);

                var jsonElement = JsonSerializer.Deserialize<JsonElement>(serializedExpectedUser);

                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.GetUser", It.IsAny<object?[]>()))
                    .ReturnsAsync(jsonElement);
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Navigation.IsRedirected", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);

                var oidcProviderOptions = new OidcProviderOptions();

                var navigationManager = new NavigationManagerStub();

                var navigateUri = string.Empty;
                navigationManager.Navigate += (sender, args) =>
                {
                    navigateUri = args.Uri;
                };

                var navigationManagerStub = new Stub<NavigationManager>(navigationManager);
                navigationManagerStub.SetField("_isInitialized", true);
                navigationManagerStub.SetField("_uri", "http://localhost:5001/fetchdata?state=c52b9dee566c480bb91a206189023d5b&session_state=e9a1349a-7d8b-433c-adeb-93176accfc17&code=3bbbd897-8496-4401-9688-4873d5d3b7b8.e9a1349a-7d8b-433c-adeb-93176accfc17.demo-app");

                using var userManager = new UserManager(new NullLogger<UserManager>(), jsRuntimeMock.Object, navigationManagerStub.Instance, oidcProviderOptions);

                _ = await userManager.GetUserAsync<User<Profile>>();

                Assert.AreEqual("http://localhost:5001/fetchdata", navigateUri);
            }
        }

        public class The_SignInRedirectAsync_Method
        {
            [Test]
            public async Task Calls_SignInRedirect_With_The_Expected_Url_Async()
            {
                var jsRuntimeMock = new Mock<IJSRuntime>();
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.SignInRedirect", It.IsAny<object?[]>()))
                    .ReturnsAsync(true);

                var navigationManagerStub = new Stub<NavigationManager>(new NavigationManagerStub());
                navigationManagerStub.SetField("_isInitialized", true);
                navigationManagerStub.SetField("_uri", "http://localhost:5000/counter");
                navigationManagerStub.SetField("_baseUri", new Uri("http://localhost:5000/"));

                using var userManager = new UserManager(new NullLogger<UserManager>(), jsRuntimeMock.Object, navigationManagerStub.Instance, new OidcProviderOptions());
                await userManager.SignInRedirectAsync("/fetchdata");

#pragma warning disable IDISP013 // Await in using
                jsRuntimeMock.Verify(runtime => runtime.InvokeAsync<object?>("BlorcOidc.Client.UserManager.SignInRedirect", It.Is<object?[]>(objects => objects.Contains("http://localhost:5000/fetchdata"))));
#pragma warning restore IDISP013 // Await in using
            }
        }
    }
}
