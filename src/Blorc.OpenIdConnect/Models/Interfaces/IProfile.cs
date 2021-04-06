namespace Blorc.OpenIdConnect
{
    public interface IProfile
    {
        string Email { get; }

        bool EmailVerified { get; }

        string FamilyName { get; }

        string GivenName { get; }

        string Name { get; }

        string PreferredUsername { get; }

        string[] Roles { get; }
    }
}
