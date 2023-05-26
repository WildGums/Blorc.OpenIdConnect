namespace Blorc.OpenIdConnect.JSInterop
{
    public interface IPromiseHandler
    {
        int Id { get; }
        void SetResult(string json);
    }
}
