using System.Linq;
using System.Threading.Tasks;
using CsCheck;

namespace Finance.Accounting.Test;


public class AmountTest
{
    [Test]
    public async Task AlwaysHasValue()
    {
        await Gen.String[1, 5].SampleAsync(async commodity =>
        {
            var amount = new Amount();

            await Assert.That(amount[commodity]).IsEqualTo(0);
        });
    }

    [Test]
    public async Task Negation()
    {
        await FinanceGen.Amount.SampleAsync(async amount =>
        {
            var negation = -amount;

            foreach (var commodity in amount.Commodities)
            {
                await Assert.That(negation[commodity]).IsEqualTo(-amount[commodity]);
            }
        });
    }

    [Test]
    public async Task Addition()
    {
        await FinanceGen.Amount.Array[2].SampleAsync(async amounts =>
        {
            var left = amounts[0];
            var right = amounts[1];

            var total = left + right;

            foreach (var commodity in left.Commodities.Union(right.Commodities))
            {
                await Assert.That(total[commodity]).IsEqualTo(left[commodity] + right[commodity]);
            }
        });
    }

    [Test]
    public async Task Subtraction()
    {
        await FinanceGen.Amount.Array[2].SampleAsync(async amounts =>
        {
            var left = amounts[0];
            var right = amounts[1];

            var total = left - right;

            foreach (var commodity in left.Commodities.Union(right.Commodities))
            {
                await Assert.That(total[commodity]).IsEqualTo(left[commodity] - right[commodity]);
            }
        });
    }

    [Test]
    public async Task Multiplication()
    {
        await Gen.Select(FinanceGen.Amount, Gen.Decimal[-100, 100])
            .SampleAsync(async (amount, factor) =>
            {
                var total = amount * factor;

                foreach (var commodity in amount.Commodities)
                {
                    await Assert.That(total[commodity]).IsEqualTo(amount[commodity] * factor);
                }
            });
    }
}