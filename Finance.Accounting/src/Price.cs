using System.Collections.Generic;

namespace Finance
{
    public record Price
    {
        private readonly string _commodity;
        private readonly Dictionary<string, decimal> _rates;

        public Price(string baseCommodity)
        {
            _commodity = baseCommodity;
            _rates = new();
        }

        public string BaseCommodity => _commodity;
        public ICollection<string> Commodities => _rates.Keys;

        public decimal this[string commodity]
        {
            get
            {
                return _rates[commodity];
            }
            set
            {
                _rates[commodity] = value;
            }
        }

        public static Amount operator *(Amount amount, Price price)
        {
            var result = new Amount();

            foreach (var commodity in amount.Commodities)
            {
                if (price._rates.ContainsKey(commodity))
                {
                    var rate = price._rates[commodity];

                    result.Add(price._commodity, amount[commodity] * rate);
                }
                else
                {
                    result.Add(commodity, amount[commodity]);
                }
            }

            return result;
        }
    }
}