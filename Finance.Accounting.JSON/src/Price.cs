using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    public class PriceConverter : JsonConverter<Price>
    {
        public override Price Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.AssertStartObject();

            string baseCommodity = string.Empty;
            var tempPrice = new Price(string.Empty);

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();

                if (propertyName == "base_commodity")
                {
                    reader.Read();
                    baseCommodity = reader.GetString();
                }
                else if (propertyName == "rates")
                {
                    reader.Read();
                    tempPrice = ReadRates(ref reader);
                }
            }

            reader.AssertEndObject();

            var result = new Price(baseCommodity);
            foreach (var commodity in tempPrice.Commodities)
            {
                result[commodity] = tempPrice[commodity];
            }

            return result;
        }

        private Price ReadRates(ref Utf8JsonReader reader)
        {
            var tempPrice = new Price(string.Empty);

            reader.AssertStartObject();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var commodity = reader.GetString();
                reader.Read();
                var quantity = reader.GetDecimal();

                tempPrice[commodity] = quantity;
            }

            reader.AssertEndObject();

            return tempPrice;
        }

        public override void Write(Utf8JsonWriter writer, Price value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("base_commodity", value.BaseCommodity);

            writer.WritePropertyName("rates");
            writer.WriteStartObject();

            foreach (var commodity in value.Commodities)
            {
                writer.WriteNumber(commodity, value[commodity]);
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}