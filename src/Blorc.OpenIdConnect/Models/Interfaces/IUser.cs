namespace Blorc.OpenIdConnect
{
    public interface IUser : IHasRoles
    {
        string AccessToken { get; }

        long ExpiresAt { get; }

        IProfile Profile { get; }

        string SessionState { get; }

        string TokenType { get; }

        [ObsoleteEx(ReplacementTypeOrMember = $"{nameof(HasRolesExtensions.IsInRole)}", RemoveInVersion = "2.0.0")]
        bool IsInRole(string role);
    }
}
