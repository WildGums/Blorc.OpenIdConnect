namespace Blorc.OpenIdConnect
{
    [ObsoleteEx(Message = "This will be removed in favor of custom user model", RemoveInVersion = "2.0.0")]
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
