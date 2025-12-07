using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.Accounting.JSON;

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
        var lookup = new DefaultDictionary<Account, Amount>(() => new());
        var accounts = new AccountTree();

        reader.AssertStartObject();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            var account = _accountConverter.ReadAsPropertyName(ref reader, typeof(Account), options);
            reader.Read();

            accounts.Add(account);

            var amount = _amountConverter.Read(ref reader, typeof(Amount), options);

            lookup[account] = amount!;
        }
        reader.AssertEndObject();

        var balance = new Balance();
        foreach (var account in accounts.EnumerateUp())
        {
            var diff = lookup[account] - balance[account];

            balance.Add(account, diff);
        }

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