namespace Blorc.OpenIdConnect
{
    using System.Buffers;
    using System.Text.Json;

    public static partial class JsonElementExtensions
    {
        public static TObject? ToObject<TObject>(this JsonElement element, JsonSerializerOptions? options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();

            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize<TObject>(bufferWriter.WrittenSpan, options);
        }
    }
}
