using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Finance.Accounting;

/// <summary>
/// A quantity of a commodity.
/// </summary>
public record SingleAmount
{
    internal SingleAmount(string commodity, decimal quantity)
    {
        Commodity = commodity;
        Quantity = quantity;
    }

    public string Commodity { get; private set; }
    public decimal Quantity { get; private set; }
}

/// <summary>
/// A total mapping from commodities to quantities.
/// </summary>
public record Amount : IEquatable<Amount>, IEnumerable<SingleAmount>
{
    private readonly DefaultDictionary<string, decimal> _quantities;

    public Amount()
    {
        _quantities = new(() => 0);
    }

    public Amount(string commodity, decimal quantity)
        : this()
    {
        Add(commodity, quantity);
    }

    public ICollection<string> Commodities => _quantities.Keys;

    public decimal this[string commodity]
    {
        get
        {
            return _quantities[commodity];
        }
    }

    public void Add(string commodity, decimal quantity)
    {
        _quantities[commodity] += quantity;

        if (_quantities[commodity] == 0)
        {
            _quantities.Remove(commodity);
        }
    }

    public bool IsPositive
    {
        get
        {
            var result = true;

            foreach (var pair in _quantities)
            {
                result = result && pair.Value > 0;
            }

            return result;
        }
    }

    public bool IsZero => _quantities.Count == 0;

    public virtual bool Equals(Amount? other)
    {
        if (other == null)
        {
            return false;
        }

        var leftCommodities = new HashSet<string>(Commodities);
        var rightCommodities = new HashSet<string>(other.Commodities);

        if (!leftCommodities.SetEquals(rightCommodities))
        {
            return false;
        }

        var result = true;
        foreach (var pair in _quantities)
        {
            result = result && pair.Value == other[pair.Key];
        }

        return result;
    }

    public override int GetHashCode()
    {
        var result = 0;
        foreach (var pair in _quantities)
        {
            result ^= pair.Key.GetHashCode() ^ pair.Value.GetHashCode();
        }

        return result;
    }

    public static Amount operator -(Amount amount)
    {
        var newAmount = new Amount();
        foreach (var pair in amount._quantities)
        {
            newAmount._quantities[pair.Key] = -pair.Value;
        }

        return newAmount;
    }

    public static Amount operator -(Amount left, Amount right)
    {
        return left + (-right);
    }

    public static Amount operator +(Amount left, Amount right)
    {
        var result = new Amount();

        foreach (var pair in left._quantities)
        {
            result.Add(pair.Key, pair.Value);
        }

        foreach (var pair in right._quantities)
        {
            result.Add(pair.Key, pair.Value);
        }

        return result;
    }

    public static Amount operator *(Amount left, decimal right)
    {
        var result = new Amount();

        foreach (var pair in left._quantities)
        {
            result._quantities[pair.Key] = pair.Value * right;
        }

        return result;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var pair in _quantities)
        {
            builder.AppendLine($"{pair.Key} {pair.Value}");
        }

        return builder.ToString();
    }

    public IEnumerator<SingleAmount> GetEnumerator()
    {
        foreach (var pair in _quantities)
        {
            yield return new SingleAmount(pair.Key, pair.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}