using System.Linq;
using CsCheck;
using NUnit.Framework;

namespace Finance.Accounting.Test;

public class AmountTest
{
    Gen<Amount> _amountGen;

    [SetUp]
    public void Setup()
    {
        _amountGen = Gen.Dictionary(Gen.String[1, 2], Gen.Decimal)
            .Select(x =>
            {
                var amount = new Amount();
                foreach (var entry in x)
                {
                    amount.Add(entry.Key, entry.Value);
                }

                return amount;
            });
    }

    [Test]
    public void AlwaysHasValue()
    {
        Gen.String[1, 5].Sample(commodity =>
        {
            var amount = new Amount();

            Assert.That(amount[commodity], Is.Zero);
        });
    }

    [Test]
    public void Negation()
    {
        _amountGen.Sample(amount =>
        {
            var negation = -amount;

            foreach (var commodity in amount.Commodities)
            {
                Assert.That(negation[commodity], Is.EqualTo(-amount[commodity]));
            }
        });
    }

    [Test]
    public void Addition()
    {
        _amountGen.Array[2].Sample(amounts =>
        {
            var left = amounts[0];
            var right = amounts[1];

            var total = left + right;

            foreach (var commodity in left.Commodities.Union(right.Commodities))
            {
                Assert.That(total[commodity], Is.EqualTo(left[commodity] + right[commodity]));
            }
        });
    }

    [Test]
    public void Subtraction()
    {
        _amountGen.Array[2].Sample(amounts =>
        {
            var left = amounts[0];
            var right = amounts[1];

            var total = left - right;

            foreach (var commodity in left.Commodities.Union(right.Commodities))
            {
                Assert.That(total[commodity], Is.EqualTo(left[commodity] - right[commodity]));
            }
        });
    }

    [Test]
    public void Multiplication()
    {
        Gen.Select(_amountGen, Gen.Decimal[-100, 100])
            .Sample((amount, factor) =>
            {
                var total = amount * factor;

                foreach (var commodity in amount.Commodities)
                {
                    Assert.That(total[commodity], Is.EqualTo(amount[commodity] * factor));
                }
            });
    }
}