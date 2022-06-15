namespace Blorc.OpenIdConnect
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    [ObsoleteEx(ReplacementTypeOrMember = $"{nameof(HttpClientBuilderExtensions.AddAccessToken)} or {nameof(HttpClientBuilderExtensions.CustomizeHttpRequestMessage)} extension methods", RemoveInVersion = "2.0.0")]
    public interface IHttpClient
    {
        Task<HttpResponseMessage> DeleteAsync(string requestUri);

        Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken);

        Task<HttpResponseMessage> DeleteAsync(Uri requestUri);

        Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken);

        Task<T> GetAsNewtonsoftJsonAsync<T>(string requestUri);

        Task<T> GetAsNewtonsoftJsonAsync<T>(string requestUri, JsonSerializerSettings settings);

        Task<HttpResponseMessage> GetAsync(string requestUri);

        Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption);

        Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken);

        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken);

        Task<HttpResponseMessage> GetAsync(Uri requestUri);

        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption);

        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken);

        Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken);

        Task<byte[]> GetByteArrayAsync(string requestUri);

        Task<byte[]> GetByteArrayAsync(Uri requestUri);

        Task<T> GetJsonAsync<T>(string requestUri);

        Task<T> GetJsonAsync<T>(string requestUri, JsonSerializerOptions options);

        Task<Stream> GetStreamAsync(string requestUri);

        Task<Stream> GetStreamAsync(Uri requestUri);

        Task<string> GetStringAsync(string requestUri);

        Task<string> GetStringAsync(Uri requestUri);

        Task<HttpResponseMessage> PostAsJsonAsync<TValue>(Uri requestUri, TValue value, CancellationToken cancellationToken);

        Task<HttpResponseMessage> PostAsNewtonsoftJsonAsync<TValue>(string requestUri, TValue value);

        Task<HttpResponseMessage> PostAsNewtonsoftJsonAsync<TValue>(string requestUri, TValue value, JsonSerializerSettings settings);

        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);

        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken);

        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content);

        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken);

        Task<HttpResponseMessage> PutAsJsonAsync<TValue>(Uri requestUri, TValue value, CancellationToken cancellationToken);

        Task<HttpResponseMessage> PutAsNewtonsoftJsonAsync<TValue>(string requestUri, TValue value);

        Task<HttpResponseMessage> PutAsNewtonsoftJsonAsync<TValue>(string requestUri, TValue value, JsonSerializerSettings settings);

        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);

        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken);

        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);

        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
