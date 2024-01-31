namespace Blorc.OpenIdConnect.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Json;
    using NUnit.Framework;

    [SetUpFixture]
    public class SetupTrace
    {
        [OneTimeSetUp]
        public void StartTest()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        [OneTimeTearDown]
        public void EndTest()
        {
            Trace.Flush();
        }
    }

    [TestFixture]
    public class ObjectExtensionsFacts
    {
        public class The_AsClaims_Method
        {
            [Test]
            public void Collects_Roles_AsClaims()
            {
                var user = new User<Profile>
                {
                    AccessToken = "1234567890",
                    Profile = new Profile
                    {
                        Roles = [ "Administrator", "System Administrator" ],
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

                var claims = user.AsClaims().Where(claim => claim.Type == ClaimTypes.Role).ToList();

                Assert.That(claims, Is.Not.Empty);

                foreach (var role in user.Profile.Roles)
                {
                    var claim = claims.FirstOrDefault(c => c.Value == role);
                    Assert.That(claim, Is.Not.Null);
                }
            }

            [Test]
            public void Collects_Aud_AsClaims()
            {
                var user = new User<Profile>
                {
                    AccessToken = "1234567890",
                    Profile = new Profile
                    {
                        Aud = JsonDocument.Parse("\"http://localhost\"").RootElement,
                        Roles = [ "Administrator", "System Administrator" ],
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

                var claims = user.AsClaims().Where(claim => claim.Type == "aud").ToList();
                Assert.That(claims.Count, Is.EqualTo(1));
                var claim = claims.FirstOrDefault(c => c.Value == user.Profile.Aud.GetString());
                Assert.That(claim, Is.Not.Null);
            }

            public void Collects_Aud_AsClaims_Array()
            {
                var user = new User<Profile>
                {
                    AccessToken = "1234567890",
                    Profile = new Profile
                    {
                        Aud = JsonDocument.Parse("[\"http://localhost\", \"http://example.com\"]").RootElement,
                        Roles = [ "Administrator", "System Administrator" ],
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

                var claims = user.AsClaims().Where(claim => claim.Type == "aud").ToList();
                Assert.That(claims.Count, Is.EqualTo(2));

                foreach (var audience in user.Profile.Audiences)
                {
                    var claim = claims.FirstOrDefault(c => c.Value == audience);
                    Assert.That(claim, Is.Not.Null);
                }
            }

            [Test]
            public void Collects_Claims_From_Each_Item_Of_A_Collection()
            {
                var users = new List<User<Profile>>();
                var user = new User<Profile>
                {
                    AccessToken = "1234567890",
                    Profile = new Profile
                    {
                        Aud = JsonDocument.Parse("\"http://localhost\"").RootElement,
                        Roles = [ "Administrator", "System Administrator" ],
                        Email = "jane.doe@blorc.com",
                        EmailVerified = true,
                        FamilyName = "Doe",
                        GivenName = "Jane",
                        Name = "Jane Doe",
                        PreferredUsername = "jane.doe"
                    },
                    ExpiresAt = 10,
                    SessionState = "alskjdhflaskjdhflaksjdhqwpoyir",
                    TokenType = "Bearer",
                    RefreshToken = "alskjdhflaskjdhflaksjdhqwpoyir",
                    IdToken = "alskjdhflaskjdhflaksjdhqwpoyir",
                    Scope = "openid profile email"
                };

                users.Add(user);
                users.Add(user);

                var claims = users.AsClaims().ToList();

                Assert.That(claims.Count, Is.EqualTo(28));
            }

            [Test]
            public void Collects_Claims_From_Complex_Type()
            {
                var user = new User<Profile>
                {
                    AccessToken = "1234567890",
                    Profile = new Profile
                    {
                        Roles = new[]
                        {
                            "Administrator", "System Administrator"
                        },
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

                var complexType = new ComplexType
                {
                    Uri = "http://localhost",
                    DateTime = DateTime.Now,
                    Users = new List<User<Profile>>(),
                    Ints = new List<int> { 1, 2 },
                    User = user
                };

                complexType.Users.Add(user);
                complexType.Users.Add(user);
                complexType.Users.Add(null);

                var claims = complexType.AsClaims().ToList();

                Assert.That(claims.Count, Is.EqualTo(32));
            }

            public class ComplexType
            {
                [ClaimType(ClaimTypes.Uri)]
                public string Uri { get; set; }

                [ClaimType(ClaimTypes.DateOfBirth)]
                public DateTime DateTime { get; set; }

                [ClaimType(ClaimTypes.Sid)]
                public List<int> Ints { get; set; }

                public List<User<Profile>> Users { get; set; }

                public User<Profile> User { private get; set; }
            }
        }
    }
}
