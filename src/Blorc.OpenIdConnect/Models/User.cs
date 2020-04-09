namespace Blorc.OpenIdConnect
{
    using System;
    using System.Linq;

    public class User
    {
        #region Properties
        public string IdToken { get; set; }

        public Guid SessionState { get; set; }

        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public Profile Profile { get; set; }

        public long ExpiresAt { get; set; }
        #endregion

        #region Methods
        public bool IsInRole(string role)
        {
            if (Profile?.Roles == null)
            {
                return false;
            }

            return Profile.Roles.Contains(role);
        }
        #endregion
    }
}
