using System.Threading.Tasks;
using CsCheck;

namespace Finance.Accounting.Test;


public class AccountTest
{
    [Test]
    public async Task Parent()
    {
        await FinanceGen.Account.NonRoot.SampleAsync(async account =>
        {
            var parent = account.Parent;

            await Assert.That(parent).IsNotEqualTo(account);
            await Assert.That(account.IsUnder(parent)).IsTrue();
        });
    }

    [Test]
    public async Task AccountIsUnderItself()
    {
        await FinanceGen.Account.SampleAsync(async account =>
        {
            await Assert.That(account.IsUnder(account)).IsTrue();
        });
    }

    [Test]
    public async Task RootParent()
    {
        var root = new Account();
        await Assert.That(root.Depth).IsEqualTo(0);

        var parent = root.Parent;
        await Assert.That(parent).IsEqualTo(root);
    }

    [Test]
    public async Task Equality()
    {
        await Gen.String[0, 5].Array.SampleAsync(async branches =>
        {
            var left = new Account(branches);
            var right = new Account(branches);

            await Assert.That(left).IsEqualTo(right);
        });
    }

    [Test]
    public async Task DepthIsNotNegative()
    {
        await Gen.String.Array[0, 5].SampleAsync(async branch =>
        {
            var account = new Account(branch);

            await Assert.That(account.Depth).IsEqualTo(branch.Length);
        });
    }
}