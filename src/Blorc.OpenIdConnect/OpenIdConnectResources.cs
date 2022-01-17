namespace Blorc.OpenIdConnect
{
    public static class OpenIdConnectResources
    {
        public const string DefaultSilentRefresh = "silent-refresh.html";

        public static string GetDefaultSilentRefreshPage(string baseUrl)
        {
            return baseUrl + "_content/" + typeof(OpenIdConnectResources).Assembly.GetName().Name + "/" + DefaultSilentRefresh;
        }
    }
}
