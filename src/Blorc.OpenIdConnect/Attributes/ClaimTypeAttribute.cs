namespace Blorc.OpenIdConnect
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class ClaimTypeAttribute : Attribute
    {
        public ClaimTypeAttribute(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; }
    }
}
