using Newtonsoft.Json;

namespace PieHandlerService.Core.Converters;

public sealed class WriteRawJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        if (null != value) { writer.WriteRawValue(value.ToString()); }
    }


    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) =>
        throw new NotImplementedException();

    public override bool CanConvert(Type objectType) => typeof(string).IsAssignableFrom(objectType);
    public override bool CanRead => false;
}
