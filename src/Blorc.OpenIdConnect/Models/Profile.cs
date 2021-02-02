namespace Blorc.OpenIdConnect
{
    using System;

    public class Profile
    {
        public Guid Jti { get; set; }

        public Guid Sub { get; set; }

        public string Typ { get; set; }

        public string Azp { get; set; }

        public long AuthTime { get; set; }

        public Guid SessionState { get; set; }

        public string Arc { get; set; }

        public string SHash { get; set; }

        public bool EmailVerified { get; set; }

        public string[] Roles { get; set; }

        public string Name { get; set; }

        public string PreferredUsername { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Email { get; set; }
    }
}
