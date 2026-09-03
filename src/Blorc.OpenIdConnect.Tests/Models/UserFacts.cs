namespace Blorc.OpenIdConnect.Tests;

using System.Text.Json;
using NUnit.Framework;
using Is = NUnit.DeepObjectCompare.Is;

[TestFixture]
public class UserFacts
{
    public class JsonFacts
    {
        [Test]
        public void Can_Deserialize_User()
        {
            var json = @"{
                ""access_token"": ""access_token_value"",
                ""expires_at"": 10,
                ""id_token"": ""id_token_value"",
                ""profile"": {
                    ""aud"": ""https://localhost:5001"",
                    ""roles"": [ ""Administrator"", ""System Administrator"" ],
                    ""email"": ""jane.doe@blorc.com"",
                    ""email_verified"": true,
                    ""name"": ""Jane Doe"",
                    ""preferred_username"": ""jane.doe"",
                    ""sub"": ""1234567890""
                },
                ""refresh_token"": ""refresh_token_value"",
                ""scope"": ""openid profile email roles"",
                ""session_state"": ""session_state_value"",
                ""token_type"": ""Bearer""
            }";

            var user = JsonSerializer.Deserialize<User<Profile>>(json);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.AccessToken, Is.EqualTo("access_token_value"));
            Assert.That(user.ExpiresAt, Is.EqualTo(10));
            Assert.That(user.IdToken, Is.EqualTo("id_token_value"));
            Assert.That(user.Profile, Is.Not.Null);
            Assert.That(user.Profile!.Audiences, Is.DeepEqualTo(new[] { "https://localhost:5001" }));
            Assert.That(user.Profile!.Roles, Is.DeepEqualTo(new[] { "Administrator", "System Administrator" }));
            Assert.That(user.Profile!.Email, Is.EqualTo("jane.doe@blorc.com"));
            Assert.That(user.Profile!.EmailVerified, Is.True);
            Assert.That(user.Profile!.Name, Is.EqualTo("Jane Doe"));
            Assert.That(user.Profile!.PreferredUsername, Is.EqualTo("jane.doe"));
            Assert.That(user.Profile!.Sub, Is.EqualTo("1234567890"));
            Assert.That(user.RefreshToken, Is.EqualTo("refresh_token_value"));
            Assert.That(user.Scope, Is.EqualTo("openid profile email roles"));
            Assert.That(user.Scopes, Is.DeepEqualTo(new[] { "openid", "profile", "email", "roles" }));
            Assert.That(user.SessionState, Is.EqualTo("session_state_value"));
            Assert.That(user.TokenType, Is.EqualTo("Bearer"));
        }

        [Test]
        public void Can_Deserialize_Additional_Data()
        {
            var json = @"{
                ""access_token"": ""access_token_value"",
                ""expires_at"": 10,
                ""profile"": {
                    ""sub"": ""1234567890"",
                    ""some_additional_data"": ""some_additional_data_value""
                },
                ""token_type"": ""Bearer""
            }";

            var user = JsonSerializer.Deserialize<User<Profile>>(json);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Profile!.Audiences, Is.Empty);
            Assert.That(user.Profile!.AdditionalData, Is.Not.Null);
            Assert.That(user.Profile!.AdditionalData!["some_additional_data"].GetString(), Is.EqualTo("some_additional_data_value"));
        }

        [Test]
        public void Can_Deserialize_Aud_Array()
        {
            var json = @"{
                ""access_token"": ""access_token_value"",
                ""expires_at"": 10,
                ""profile"": {
                    ""aud"": [ ""https://localhost:5001"", ""https://localhost:5002"" ],
                    ""sub"": ""1234567890""
                },
                ""token_type"": ""Bearer""
            }";

            var user = JsonSerializer.Deserialize<User<Profile>>(json);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Profile!.Audiences, Is.DeepEqualTo(new[] { "https://localhost:5001", "https://localhost:5002" }));
        }
    }
}
