﻿namespace Blorc.OpenIdConnect.Tests
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

                Assert.That(claims, Is.Not.Empty);

                foreach (var role in user.Profile.Roles)
                {
                    var claim = claims.FirstOrDefault(c => c.Value == role);
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

                Assert.That(claims.Count, Is.EqualTo(14));
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

                Assert.That(claims.Count, Is.EqualTo(18));
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
