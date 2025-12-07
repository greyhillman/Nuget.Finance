using CsCheck;


namespace Finance.Accounting.Test;


public static class FinanceGen
{
    public static readonly AccountGen Account = new();
    public static readonly AmountGen Amount = new();
}

public class AccountGen : Gen<Account>
{
    private readonly Gen<string[]> _gen;

    public AccountGen(Gen<string[]> gen)
    {
        _gen = gen;
    }

    public AccountGen() : this(Gen.String.Array)
    { }

    public AccountGen NonRoot => new(Gen.String.Array.Nonempty);

    public override Account Generate(PCG pcg, Size? min, out Size size)
    {
        return _gen
            .Select(branches => new Account(branches))
            .Generate(pcg, min, out size);
    }
}

public class AmountGen : Gen<Amount>
{
    private readonly Gen<string> _commodityGen;
    private readonly Gen<decimal> _quantityGen;

    public AmountGen()
    {
        _commodityGen = Gen.String[1, 10];
        _quantityGen = Gen.Decimal;
    }

    public override Amount Generate(PCG pcg, Size? min, out Size size)
    {
        return Gen.Dictionary(_commodityGen, _quantityGen)
            .Select(dictionary =>
            {
                var result = new Amount();
                foreach (var pair in dictionary)
                {
                    var commodity = pair.Key;
                    var quantity = pair.Value;

                    var amount = new Amount(commodity, quantity);

                    result += amount;
                }

                return result;
            })
            .Generate(pcg, min, out size);
    }
}
