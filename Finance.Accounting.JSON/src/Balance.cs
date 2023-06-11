using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.JSON
{
    public class ObjectBalanceConverter : JsonConverter<Balance>
    {
        private readonly JsonConverter<Account> _accountConverter;
        private readonly JsonConverter<Amount> _amountConverter;

        public ObjectBalanceConverter(JsonConverter<Account> accountConverter, JsonConverter<Amount> amountsConverter)
        {
            _accountConverter = accountConverter;
            _amountConverter = amountsConverter;
        }

        public override Balance? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.AssertStartObject();

            var balance = new Balance();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var account = _accountConverter.ReadAsPropertyName(ref reader, typeof(Account), options);
                reader.Read();

                var amount = _amountConverter.Read(ref reader, typeof(Amount), options);

                balance.Add(account, amount);
            }
            reader.AssertEndObject();

            return balance;
        }

        public override void Write(Utf8JsonWriter writer, Balance value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var account in value.Accounts.OrderBy(x => x))
            {
                var amount = value[account];

                _accountConverter.WriteAsPropertyName(writer, account, options);
                _amountConverter.Write(writer, amount, options);
            }

            writer.WriteEndObject();
        }
    }
}