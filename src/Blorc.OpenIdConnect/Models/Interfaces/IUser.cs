namespace Blorc.OpenIdConnect
{
    public interface IUser
    {
        string AccessToken { get; }

        long ExpiresAt { get; }

        IProfile Profile { get; }

        string SessionState { get; }

        string TokenType { get; }

        bool IsInRole(string role);
    }
}
