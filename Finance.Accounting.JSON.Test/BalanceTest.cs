using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.Accounting.JSON.Test;

public class BalanceTest
{
    private readonly JsonSerializerOptions _jsonOptions;

    public BalanceTest()
    {
        var accountConverter = new ArrayAccountConverter();
        var amountConverter = new AmountConverter();
        var balanceConverter = new ObjectBalanceConverter(accountConverter, amountConverter);

        _jsonOptions = new JsonSerializerOptions()
        {
            Converters = {
                accountConverter,
                amountConverter,
                balanceConverter,
            }
        };
    }

    private T WrapUnwrap<T>(T value)
    {
        using (var stream = new MemoryStream())
        using (var reader = new StreamReader(stream))
        {
            JsonSerializer.Serialize(stream, value, _jsonOptions);
            stream.Seek(0, SeekOrigin.Begin);

            var result = JsonSerializer.Deserialize<T>(stream, _jsonOptions);

            Assert.NotNull(result);

            return result;
        }
    }

    [Fact]
    public void SingleAccount()
    {
        var assets = new Account("assets");
        var balance = new Balance();

        balance.Add(assets, new Amount("CAD", 100));

        balance = WrapUnwrap(balance);

        Assert.Equal(100, balance[assets]["CAD"]);
    }

    [Fact]
    public void MultipleAccounts()
    {
        var assets = new Account("assets");
        var liabilities = new Account("liabilities");
        var balance = new Balance();

        balance.Add(assets, new Amount("CAD", 100));
        balance.Add(liabilities, new Amount("CAD", -100));

        balance = WrapUnwrap(balance);

        Assert.Multiple(() =>
        {
            Assert.Equal(100, balance[assets]["CAD"]);
            Assert.Equal(-100, balance[liabilities]["CAD"]);
        });
    }

    [Fact]
    public void NestedAccounts()
    {
        var assets = new Account("assets");
        var bank = new Account("assets", "bank");
        var savings = new Account("assets", "bank", "savings");
        var chequing = new Account("assets", "bank", "chequing");
        var cash = new Account("assets", "cash");

        var balance = new Balance();
        balance.Add(savings, new Amount("CAD", 100));
        balance.Add(chequing, new Amount("CAD", 100));
        balance.Add(cash, new Amount("CAD", 50));

        balance = WrapUnwrap(balance);

        Assert.Multiple(() =>
        {
            Assert.Equal(100, balance[savings]["CAD"]);
            Assert.Equal(100, balance[chequing]["CAD"]);
            Assert.Equal(200, balance[bank]["CAD"]);
            Assert.Equal(50, balance[cash]["CAD"]);
            Assert.Equal(250, balance[assets]["CAD"]);
        });
    }

    [Fact]
    public void NegativeAccounts()
    {
        var income = new Account("income");
        var assets = new Account("assets");

        var balance = new Balance();
        var amount = new Amount("CAD", 100);
        balance.Add(income, -amount);
        balance.Add(assets, amount);

        balance = WrapUnwrap(balance);

        Assert.Multiple(() =>
        {
            Assert.Equal(-100, balance[income]["CAD"]);
            Assert.Equal(100, balance[assets]["CAD"]);
        });
    }
}