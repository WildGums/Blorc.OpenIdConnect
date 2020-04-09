namespace Blorc.OpenIdConnect
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class HttpClientExtensions
    {
        #region Methods
        public static void SetBearerToken(this HttpClient @this, string token)
        {
            @this.SetToken("Bearer", token);
        }

        public static void SetToken(this HttpClient @this, string scheme, string token)
        {
            // Argument.IsNotNull(() => @this);
            // Argument.IsNotNullOrWhitespace(() => scheme);
            // Argument.IsNotNullOrWhitespace(() => token);

            @this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }
        #endregion
    }
}
