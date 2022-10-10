namespace Blorc.OpenIdConnect
{
    using System;
    using System.Buffers;
    using System.Text.Json;

    public static class JsonElementExtensions
    {
        public static TObject? ToObject<TObject>(this JsonElement element, JsonSerializerOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(element);

            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize<TObject>(bufferWriter.WrittenSpan, options);
        }
    }
}
