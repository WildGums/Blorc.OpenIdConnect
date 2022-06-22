namespace Blorc.OpenIdConnect.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using NUnit.Framework;

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

                var claims = user.AsClaims().Where(claim => claim.Type == ClaimTypes.Role).ToList();

                Assert.IsNotEmpty(claims);
                foreach (var claim in claims)
                {
                    Assert.Contains(claim.Value, user.Profile.Roles);
                }
            }


            [Test]
            public void Collects_Claims_From_Each_Item_Of_A_Collection()
            {
                List<User<Profile>> users = new List<User<Profile>>();
                var user = new User<Profile>
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

                users.Add(user);
                users.Add(user);

                var claims = users.AsClaims().ToList();

                Assert.AreEqual(12, claims.Count);
            }

            [Test]
            public void Collects_Claims_From_Complex_Type()
            {
                var complexType = new ComplexType
                {
                    Uri = "http://localhost",
                    DateTime = DateTime.Now,
                    Users = new List<User<Profile>>(),
                    Ints = new List<int> { 1, 2 }
                };

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

                complexType.Users.Add(user);
                complexType.Users.Add(user);

                var claims = complexType.AsClaims().ToList();

                Assert.AreEqual(16, claims.Count);
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
            }
        }
    }
}
