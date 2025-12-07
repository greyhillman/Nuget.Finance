using System.Linq;
using System.Threading.Tasks;
using CsCheck;


namespace Finance.Accounting.Test;


public class BalanceTest
{
    [Test]
    public async Task WorksForAllAccounts()
    {
        await FinanceGen.Account.SampleAsync(async account =>
        {
            var balance = new Balance();

            await Assert.That(balance[account]).Satisfies(amount => amount!.IsZero);
        });
    }

    [Test]
    public async Task HasTreeStructure()
    {
        await Gen.Select(FinanceGen.Account, FinanceGen.Amount)
            .SampleAsync(async (account, amount) =>
            {
                var balance = new Balance();

                balance.Add(account, amount);

                using (Assert.Multiple())
                {
                    for (var current = account; current.Parent != current; current = current.Parent)
                    {
                        await Assert.That(balance[current]).IsEqualTo(amount);
                    }
                }
            });
    }

    [Test]
    public async Task AccumulatesInAccount()
    {
        await Gen.Select(FinanceGen.Account, FinanceGen.Amount, FinanceGen.Amount)
        .SampleAsync(async (account, initial, addition) =>
        {
            var balance = new Balance();

            balance.Add(account, initial);
            balance.Add(account, addition);

            using (Assert.Multiple())
            {
                for (var current = account; current.Parent != current; current = current.Parent)
                {
                    await Assert.That(balance[current]).IsEqualTo(initial + addition);
                }
            }
        });
    }

    //     [Test]
    //     public void AddingBalancesIsSameAsOne()
    //     {
    //         Gen.Select(_accountGen, _amountGen, _accountGen, _amountGen)
    //             .Sample((leftAccount, leftAmount, rightAccount, rightAmount) => {
    //                 var left = new Balance();
    //                 left.Add(leftAccount, leftAmount);
    // 
    //                 var right = new Balance();
    //                 right.Add(rightAccount, rightAmount);
    // 
    //                 var total = new Balance();
    //                 total.Add(leftAccount, leftAmount);
    //                 total.Add(rightAccount, rightAmount);
    // 
    //                 var addition = left + right;
    // 
    //                 Assert.That(addition[leftAccount], Is.EqualTo(total[leftAccount]));
    //                 Assert.That(addition[rightAccount], Is.EqualTo(total[rightAccount]));
    //             });
    //     }
}