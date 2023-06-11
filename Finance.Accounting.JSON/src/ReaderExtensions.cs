using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    internal static class JsonReaderExtensions
    {
        public static void AssertStartArray(this Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }
        }

        public static void AssertEndArray(this Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException();
            }
        }

        public static void AssertStartObject(this Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
        }

        public static void AssertEndObject(this Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }
        }
    }

    public class DateDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetDateTime();

            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var date = value.ToString("yyyy-MM-dd");

            writer.WriteStringValue(date);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var date = value.ToString("yyyy-MM-dd");

            writer.WritePropertyName(date);
        }
    }
}
