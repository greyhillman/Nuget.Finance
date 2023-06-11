using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    public class ArrayAccountConverter : JsonConverter<Account>
    {
        public override Account Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var parts = new List<string>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var part = reader.GetString();

                parts.Add(part);
            }

            return new Account(parts.ToArray());
        }

        public override void Write(Utf8JsonWriter writer, Account value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var level in value.Branch)
            {
                writer.WriteStringValue(level);
            }

            writer.WriteEndArray();
        }

        public override Account ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var inlineAccount = reader.GetString();

            // Technically incorrect as the parts may contain ":"
            return new Account(inlineAccount.Split(":"));
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, Account value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(string.Join(":", value.Branch));
        }
    }
}