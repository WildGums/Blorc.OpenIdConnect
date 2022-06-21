namespace Blorc.OpenIdConnect.Tests.Services
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Blorc.OpenIdConnect;
    using Blorc.Services;
    using Microsoft.AspNetCore.Components;
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
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsAuthenticated", Array.Empty<object>())).ReturnsAsync(true);

                var jsonElement = JsonSerializer.Deserialize<JsonElement>("{\r\n  \"access_token\": \"1234567890\"\r\n}");

                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<JsonElement>("BlorcOidc.Client.UserManager.GetUser", Array.Empty<object>())).ReturnsAsync(jsonElement);

                var oidcProviderOptions = new OidcProviderOptions();

                var navigationManagerStub = new Stub<NavigationManager>(new NavigationManagerStub());
                navigationManagerStub.SetField("_isInitialized", true);
                navigationManagerStub.SetField("_uri", "http://localhost");

                var userManager = new UserManager(jsRuntimeMock.Object, navigationManagerStub.Instance, new Mock<IConfigurationService>().Object, oidcProviderOptions);

                var user = await userManager.GetUserAsync<User<Profile>>();

                Assert.IsNotNull(user);
            }

            [Test]
            public async Task Returns_An_Instance_Of_The_Specified_User_Type_With_The_Expected_Values_In_All_Properties_Async()
            {
                var jsRuntimeMock = new Mock<IJSRuntime>();
                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<bool>("BlorcOidc.Client.UserManager.IsAuthenticated", Array.Empty<object>())).ReturnsAsync(true);

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

                jsRuntimeMock.Setup(runtime => runtime.InvokeAsync<JsonElement>("BlorcOidc.Client.UserManager.GetUser", Array.Empty<object>())).ReturnsAsync(jsonElement);

                var oidcProviderOptions = new OidcProviderOptions();

                var navigationManagerStub = new Stub<NavigationManager>(new NavigationManagerStub());
                navigationManagerStub.SetField("_isInitialized", true);
                navigationManagerStub.SetField("_uri", "http://localhost");

                var userManager = new UserManager(jsRuntimeMock.Object, navigationManagerStub.Instance, new Mock<IConfigurationService>().Object, oidcProviderOptions);

                var user = await userManager.GetUserAsync<User<Profile>>();

                Assert.That(user, Is.DeepEqualTo(expectedUser));
            }
        }
    }
}
