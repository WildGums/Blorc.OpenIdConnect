namespace Blorc.OpenIdConnect
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Blorc.Services;

    public static class HttpClientExtensions
    {
        public static void SetBearerToken(this HttpClient @this, string token)
        {
            ArgumentNullException.ThrowIfNull(@this);
            ArgumentNullException.ThrowIfNull(token);

            @this.SetToken("Bearer", token);
        }

        public static void SetToken(this HttpClient @this, string scheme, string token)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (!string.IsNullOrWhiteSpace(scheme) && !string.IsNullOrWhiteSpace(token))
            {
                @this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            }
        }
    }
}
