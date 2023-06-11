using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    public class AmountConverter : JsonConverter<Amount>
    {
        public override Amount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var amount = new Amount();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var commodity = reader.GetString();
                reader.Read();

                var quantity = reader.GetDecimal();

                amount.Add(commodity, quantity);
            }

            return amount;
        }

        public override void Write(Utf8JsonWriter writer, Amount value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // Should include option to print commodities in order
            foreach (var commodity in value.Commodities.OrderBy(x => x))
            {
                var quantity = value[commodity];

                writer.WriteNumber(commodity, quantity);
            }

            writer.WriteEndObject();
        }
    }
}