using System.Linq;
using CsCheck;
using NUnit.Framework;

namespace Finance.Accounting.Test;

public class BalanceTest
{
    Gen<Amount> _amountGen;
    Gen<Account> _accountGen;

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

        _accountGen = Gen.String.Array[1, 4]
            .Select(branch => new Account(branch));
    }

    [Test]
    public void WorksForAllAccounts()
    {
        _accountGen.Sample(account =>
        {
            var balance = new Balance();

            Assert.That(balance[account].IsZero);
        });
    }

    [Test]
    public void HasTreeStructure()
    {
        Gen.Select(_accountGen, _amountGen)
            .Sample((account, amount) =>
            {
                var balance = new Balance();

                balance.Add(account, amount);

                for (var current = account; current.Parent != current; current = current.Parent)
                {
                    Assert.That(balance[current], Is.EqualTo(amount));
                }
            });
    }

    [Test]
    public void AccumulatesInAccount()
    {
        Gen.Select(_accountGen, _amountGen, _amountGen)
        .Sample((account, initial, addition) =>
        {
            var balance = new Balance();

            balance.Add(account, initial);
            balance.Add(account, addition);

            for (var current = account; current.Parent != current; current = current.Parent)
            {
                Assert.That(balance[current], Is.EqualTo(initial + addition));
            }
        });
    }

    [Test]
    public void AddingBalancesIsSameAsOne()
    {
        Gen.Select(_accountGen, _amountGen, _accountGen, _amountGen)
            .Sample((leftAccount, leftAmount, rightAccount, rightAmount) => {
                var left = new Balance();
                left.Add(leftAccount, leftAmount);

                var right = new Balance();
                right.Add(rightAccount, rightAmount);

                var total = new Balance();
                total.Add(leftAccount, leftAmount);
                total.Add(rightAccount, rightAmount);

                var addition = left + right;

                Assert.That(addition[leftAccount], Is.EqualTo(total[leftAccount]));
                Assert.That(addition[rightAccount], Is.EqualTo(total[rightAccount]));
            });
    }
}