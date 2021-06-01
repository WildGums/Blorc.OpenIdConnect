namespace Blorc.OpenIdConnect
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class HttpClientExtensions
    {
        public static void SetBearerToken(this HttpClient @this, string token)
        {
            @this.SetToken("Bearer", token);
        }

        public static void SetToken(this HttpClient @this, string scheme, string token)
        {
            if (!string.IsNullOrWhiteSpace(scheme) && !string.IsNullOrWhiteSpace(token))
            {
                @this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            }
        }
    }
}
