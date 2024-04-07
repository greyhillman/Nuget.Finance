using System;
using System.Linq;
using CsCheck;
using NUnit.Framework;

namespace Finance.Accounting.Test;

public class PriceTest
{
    Gen<Amount> _amountGen;
    Gen<string> _commodityGen;

    [SetUp]
    public void Setup()
    {
        _commodityGen = Gen.Char.AlphaNumeric.Array[1, 2].Select(x => new string(x));
        _amountGen =
            from length in Gen.Int[1, 10]
            from commodities in _commodityGen.ArrayUnique[length]
            // Can't allow 0's as that's the same as not having it
            from quantities in Gen.Decimal[1, 100_000].Array[length]
            select Create(length, commodities, quantities);
    }

    private static Amount Create(int length, string[] commodities, decimal[] quantities)
    {
        var amount = new Amount();

        for (var i = 0; i < length; i++)
        {
            amount.Add(commodities[i], quantities[i]);
        }

        return amount;
    }

    [Test]
    public void AddingZeroChangesNothing()
    {
        var gen =
            from amount in _amountGen
            from commodity in _commodityGen
            where !amount.Commodities.Contains(commodity)
            select new
            {
                Amount = amount,
                Commodity = commodity,
            };

        gen.Sample(data =>
        {
            var beforeCount = data.Amount.Commodities.Count;

            data.Amount.Add(data.Commodity, 0);

            Assert.That(data.Amount.Commodities.Count, Is.EqualTo(beforeCount));
        });
    }

    [Test]
    public void NewCommodity()
    {
        var gen =
            from amount in _amountGen
            from fromCommodity in Gen.OneOfConst(amount.Commodities.ToArray())
            from toCommodity in _commodityGen
            where !amount.Commodities.Contains(toCommodity)
            from rate in Gen.Decimal[-5, 5]
            select new
            {
                Amount = amount,
                Price = new Price(fromCommodity, toCommodity, rate)
            };
        gen.Sample(data =>
            {
                var amount = data.Amount;
                var price = data.Price;

                var result = amount * price;

                Assert.That(result[price.From], Is.Zero);
                Assert.That(result[price.To], Is.EqualTo(amount[price.From] * price.Rate));
            });
    }

    [Test]
    public void ExistingCommodity()
    {
        var gen =
            from amount in _amountGen
            let commodities = amount.Commodities.ToArray()
            from fromCommodity in Gen.OneOfConst(commodities)
            from toCommodity in Gen.OneOfConst(commodities)
            where fromCommodity != toCommodity
            from rate in Gen.Decimal[-5, 5]
            select new
            {
                Amount = amount,
                Price = new Price(fromCommodity, toCommodity, rate),
            };

        gen.Sample(data =>
            {
                var amount = data.Amount;
                var price = data.Price;

                var result = amount * price;

                var expectedQuantity = amount[price.From] * price.Rate + amount[price.To];
                Assert.That(result[price.To], Is.EqualTo(expectedQuantity));
            });
    }

    [Test]
    public void SameCommodity()
    {
        var gen =
            from amount in _amountGen
            from commodity in Gen.OneOfConst(amount.Commodities.ToArray())
            from rate in Gen.Decimal[-5, 5]
            select new
            {
                Amount = amount,
                Price = new Price(commodity, commodity, rate)
            };
        gen.Sample(data =>
            {
                var amount = data.Amount;
                var price = data.Price;

                var result = amount * price;

                var expectedQuantity = amount[price.From] * price.Rate;
                Assert.That(result[price.To], Is.EqualTo(expectedQuantity));
            });
    }

    [Test]
    public void ShouldFail()
    {
        var gen =
            from array in Gen.Int.Array.Nonempty
            from number in Gen.OneOfConst(array)
            select number;
        gen.Sample(number =>
        {
            Assert.That(true);
        });
    }
}