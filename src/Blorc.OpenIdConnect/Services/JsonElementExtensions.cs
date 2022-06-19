namespace Blorc.OpenIdConnect
{
    using System.Text.Json;

    public static class JsonElementExtensions
    {
        public static TObject ToObject<TObject>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var rawText = element.GetRawText();
            return JsonSerializer.Deserialize<TObject>(rawText, options);
        }
    }
}
