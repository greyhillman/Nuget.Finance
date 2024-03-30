using System.Collections.Generic;

namespace Finance
{
    public readonly record struct Price
    {
        public string From { get; init; }
        public string To { get; init; }
        public decimal Rate { get; init; }

        public Price(string from, string to, decimal rate)
        {
            From = from;
            To = to;
            Rate = rate;
        }

        public static Amount operator *(Amount amount, Price price)
        {
            var result = new Amount();

            foreach (var commodity in amount.Commodities)
            {
                if (commodity == price.From)
                {
                    result.Add(price.To, price.Rate * amount[commodity]);
                }
                else
                {
                    result.Add(commodity, amount[commodity]);
                }
            }

            return result;
        }

        public static Amount operator *(Price price, Amount amount) {
            return amount * price;
        }
    }
}
