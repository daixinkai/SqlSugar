using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

//namespace System.Text.Json
namespace SqlSeverTest
{
    [System.Text.Json.Serialization.JsonConverter(typeof(SystemTextJsonDbValueConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(NewtonsoftJsonDbValueConverter))]
    public struct JsonDbValue
    {

        internal class SystemTextJsonDbValueConverter : System.Text.Json.Serialization.JsonConverter<JsonDbValue>
        {
            public override JsonDbValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return default;
                }
                return reader.GetString();
            }

            public override void Write(Utf8JsonWriter writer, JsonDbValue value, JsonSerializerOptions options)
            {
                if (string.IsNullOrWhiteSpace(value._value))
                {
                    writer.WriteNullValue();
                }
                else
                {
                    writer.WriteRawValue(value._value);
                }
            }
        }

        internal class NewtonsoftJsonDbValueConverter : Newtonsoft.Json.JsonConverter<JsonDbValue>
        {
            public override JsonDbValue ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, JsonDbValue existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (reader.TokenType == Newtonsoft.Json.JsonToken.Null)
                {
                    return default;
                }
                return JToken.ReadFrom(reader).ToString(Newtonsoft.Json.Formatting.None);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, JsonDbValue value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (string.IsNullOrWhiteSpace(value._value))
                {
                    //writer.WriteNull();
                    writer.WriteRaw(null);
                }
                else
                {
                    writer.WriteRawValue(value._value);
                }
            }
        }


        public JsonDbValue()
        {
        }

        private JsonDbValue(string value)
        {
            _value = value;
        }

        private readonly string? _value;

        private JsonElement? _jsonElement;
        public JsonElement? JsonElement => _jsonElement ??= (string.IsNullOrWhiteSpace(_value) ? null : JsonSerializer.Deserialize<JsonElement>(_value));

        private JObject? _jObject;
        public JObject? JObject => _jObject ??= (string.IsNullOrWhiteSpace(_value) ? null : JObject.Parse(_value));

        private static JsonDbValue ParseFromObject(object value) => new JsonDbValue(JsonSerializer.Serialize(value));

        private T? ToValue<T>() => string.IsNullOrWhiteSpace(_value) ? default : JsonSerializer.Deserialize<T>(_value);

        public static implicit operator JsonDbValue(JsonElement? value) => value.HasValue ? new JsonDbValue(value.Value.GetRawText()) : default;
        public static implicit operator JsonDbValue(string[]? value) => value != null ? ParseFromObject(value) : default;
        public static implicit operator JsonDbValue(long[]? value) => value != null ? ParseFromObject(value) : default;
        public static implicit operator JsonDbValue(int[]? value) => value != null ? ParseFromObject(value) : default;
        public static implicit operator JsonDbValue(JObject? value) => value != null ? new JsonDbValue(value.ToString()) : default;
        public static implicit operator JsonDbValue(string? value) => value != null ? new JsonDbValue(value) : default;

        public static implicit operator JsonElement?(JsonDbValue value) => value.JsonElement;
        public static implicit operator JObject?(JsonDbValue value) => value.JObject;
        public static implicit operator string?(JsonDbValue value) => value.ToString();
        public static implicit operator string[]?(JsonDbValue value) => value.ToValue<string[]>();
        public static implicit operator long[]?(JsonDbValue value) => value.ToValue<long[]>();
        public static implicit operator int[]?(JsonDbValue value) => value.ToValue<int[]>();

        public override string? ToString() => _value;

        public static JsonDbValue EmptyObject() => new JsonDbValue("{}");

        public static JsonDbValue EmptyArray() => new JsonDbValue("[]");

    }


}
