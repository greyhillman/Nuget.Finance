using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    public class MovementConverter : JsonConverter<Movement>
    {
        private readonly JsonConverter<Account> _accountConverter;
        private readonly JsonConverter<Amount> _amountConverter;

        public MovementConverter(JsonConverter<Account> accountConverter, JsonConverter<Amount> amountConverter)
        {
            _accountConverter = accountConverter;
            _amountConverter = amountConverter;
        }

        public override Movement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            Account? from = null;
            Account? to = null;
            Amount? amount = null;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "from":
                        from = _accountConverter.Read(ref reader, typeof(Account), options);
                        break;
                    case "to":
                        to = _accountConverter.Read(ref reader, typeof(Account), options);
                        break;
                    case "amount":
                        amount = _amountConverter.Read(ref reader, typeof(Amount), options);
                        break;
                    default:
                        break;
                }
            }

            if (from.HasValue && to.HasValue && amount != null)
            {
                return new Movement(from.Value, to.Value, amount);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Movement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("from");
            _accountConverter.Write(writer, value.From, options);

            writer.WritePropertyName("to");
            _accountConverter.Write(writer, value.To, options);

            writer.WritePropertyName("amount");
            _amountConverter.Write(writer, value.Amount, options);

            writer.WriteEndObject();
        }
    }
}