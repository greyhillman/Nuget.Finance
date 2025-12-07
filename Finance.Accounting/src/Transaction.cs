using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Finance.Accounting;

/// <summary>
/// A collection of <see cref="Movement" />.
/// </summary>
public class Transaction : IEnumerable<Movement>, IEquatable<Transaction>
{
    private readonly IList<Movement> _movements;

    public int Count => _movements.Count;

    public Transaction()
    {
        _movements = new List<Movement>();
    }

    public void Move(Account from, Account to, Amount amount)
    {
        if (!amount.IsPositive)
        {
            throw new System.ArgumentException($"Must be positive amount: {amount}", nameof(amount));
        }

        if (amount.IsZero)
        {
            return;
        }

        _movements.Add(new Movement(from, to, amount));
    }

    public void Add(Transaction other)
    {
        foreach (var movement in other)
        {
            Move(movement.From, movement.To, movement.Amount);
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var movement in this)
        {
            builder.AppendLine($"{movement.From} --- {movement.Amount} --> {movement.To}");
        }

        return builder.ToString();
    }

    public bool Equals(Transaction? other)
    {
        if (other == null)
        {
            return false;
        }

        if (this.Count != other.Count)
        {
            return false;
        }

        var otherMovements = new HashSet<Movement>(other);
        foreach (var movement in this)
        {
            var didExist = otherMovements.Remove(movement);

            if (!didExist)
            {
                return false;
            }
        }

        return otherMovements.Count == 0;
    }

    public override bool Equals(object? obj)
    {
        return obj is Transaction transaction && Equals(transaction);
    }

    public IEnumerator<Movement> GetEnumerator()
    {
        return _movements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public override int GetHashCode()
    {
        var result = base.GetHashCode();

        foreach (var movement in this)
        {
            result ^= movement.GetHashCode();
        }

        return result;
    }
}

/// <summary>
/// A transfer of an amount from one account to another.
/// </summary>
/// 
/// This is the fundametal idea of double-entry bookkeeping.
public readonly record struct Movement
{
    public Account From { get; init; }
    public Account To { get; init; }
    public Amount Amount { get; init; }

    public Movement(Account from, Account to, Amount amount)
    {
        From = from;
        To = to;
        Amount = amount;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.AppendLine("Movement");
        builder.Append("  ");
        builder.AppendLine(From.ToString());
        builder.Append("  ");
        builder.AppendLine(To.ToString());
        builder.Append("  ");
        builder.AppendLine(Amount.ToString());

        return builder.ToString();
    }
}