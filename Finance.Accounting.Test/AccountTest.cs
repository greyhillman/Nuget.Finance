using System;
using CsCheck;
using NUnit.Framework;

namespace Finance.Accounting.Test;

public class AccountTest
{
    [Test]
    public void Parent()
    {
        var accountGen =
            from branch in Gen.String.Array.Nonempty
            select new Account(branch);
        
        accountGen.Sample(account =>
        {
            var parent = account.Parent;

            Assert.That(parent, Is.Not.EqualTo(account));
            Assert.That(account.IsUnder(parent));
        });
    }

    [Test]
    public void RootParent()
    {
        var root = new Account();
        Assert.That(root.Depth, Is.EqualTo(0));

        var parent = root.Parent;
        Assert.That(parent, Is.EqualTo(root));
    }

    [Test]
    public void Equality()
    {
        Gen.String[0, 5].Array.Sample(branch =>
        {
            Assert.That(branch, Is.All.Not.Null);

            var left = new Account(branch);
            var right = new Account(branch);

            Assert.That(left, Is.EqualTo(right));
        });
    }

    [Test]
    public void DepthIsNotNegative()
    {
        Gen.String.Array[0, 5].Sample(branch =>
        {
            var account = new Account(branch);

            Assert.That(account.Depth, Is.EqualTo(branch.Length));
        });
    }
}