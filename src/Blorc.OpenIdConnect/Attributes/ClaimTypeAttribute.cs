namespace Blorc.OpenIdConnect
{
    using System;

    public class ClaimTypeAttribute : Attribute
    {
        public string ClaimType { get; }

        public ClaimTypeAttribute(string claimType)
        {
            ClaimType = claimType;
        }
    }
}
