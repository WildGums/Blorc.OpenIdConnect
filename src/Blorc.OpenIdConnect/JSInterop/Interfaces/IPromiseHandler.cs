namespace Blorc.OpenIdConnect.JSInterop
{
    internal interface IPromiseHandler
    {
        int Id { get; }
        void SetResult(string json);
    }
}
