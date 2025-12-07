using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.Accounting.JSON;

public class AccountFlowConverter : JsonConverter<AccountFlow>
{
    private readonly JsonConverter<Account> _accountConverter;
    private readonly JsonConverter<Amount> _amountConverter;

    public AccountFlowConverter(JsonConverter<Account> accountConverter, JsonConverter<Amount> amountsConverter)
    {
        _accountConverter = accountConverter;
        _amountConverter = amountsConverter;
    }

    public override AccountFlow? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var flow = new AccountFlow();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
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
                    case "amounts":
                        amount = _amountConverter.Read(ref reader, typeof(Amount), options);
                        break;
                    default:
                        break;
                }
            }

            if (from.HasValue && to.HasValue && amount != null)
            {
                flow.Add(new Movement(from.Value, to.Value, amount));
            }
        }

        return flow;
    }

    public override void Write(Utf8JsonWriter writer, AccountFlow value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var source in value.Sources)
        {
            foreach (var sink in value.GetSinks(source))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("from");
                _accountConverter.Write(writer, source, options);

                writer.WritePropertyName("to");
                _accountConverter.Write(writer, sink, options);

                writer.WritePropertyName("amounts");
                _amountConverter.Write(writer, value.GetFlows(source, sink), options);

                writer.WriteEndObject();
            }
        }

        writer.WriteEndArray();
    }
}