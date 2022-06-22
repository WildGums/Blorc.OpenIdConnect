namespace Blorc.OpenIdConnect
{
    using System;

    public class ClaimTypeAttribute : Attribute
    {
        public ClaimTypeAttribute(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; }
    }
}
