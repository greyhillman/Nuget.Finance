using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    public class TransactionConverter : JsonConverter<Transaction>
    {
        private readonly JsonConverter<Movement> _movementConverter;

        public TransactionConverter(JsonConverter<Movement> movementConverter)
        {
            _movementConverter = movementConverter;
        }

        public override Transaction? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var transaction = new Transaction();

            reader.AssertStartArray();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var movement = _movementConverter.Read(ref reader, typeof(Movement), options);

                transaction.Move(movement.From, movement.To, movement.Amount);
            }
            reader.AssertEndArray();

            return transaction;
        }

        public override void Write(Utf8JsonWriter writer, Transaction value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var movement in value)
            {
                _movementConverter.Write(writer, movement, options);
            }
            writer.WriteEndArray();
        }
    }
}