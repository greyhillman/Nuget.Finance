using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    public class PriceConverter : JsonConverter<Price>
    {
        public override Price Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string from = null;
            string to = null;
            decimal? rate = null;

            reader.AssertStartObject();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();

                switch (propertyName)
                {
                    case "from":
                        reader.Read();
                        from = reader.GetString();
                        break;
                    case "to":
                        reader.Read();
                        to = reader.GetString();
                        break;
                    case "rate":
                        reader.Read();
                        rate = reader.GetDecimal();
                        break;
                    default:
                        throw new JsonException($"Unknown property: {propertyName}");
                }
            }

            reader.AssertEndObject();

            if (from == null)
            {
                throw new JsonException("Need 'from' property");
            }
            if (to == null)
            {
                throw new JsonException("Need 'to' property");
            }
            if (rate == null)
            {
                throw new JsonException("Need 'rate' property");
            }

            return new Price(from, to, rate.Value);
        }

        public override void Write(Utf8JsonWriter writer, Price value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("from", value.From);
            writer.WriteString("to", value.To);
            writer.WriteNumber("rate", value.Rate);

            writer.WriteEndObject();
        }
    }
}