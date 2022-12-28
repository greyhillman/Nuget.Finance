using System.Collections.Generic;
using System.Text;

namespace Finance
{
    public class Balance
    {
        private readonly DefaultDictionary<Account, Amount> _lookup;

        public Balance()
        {
            _lookup = new(() => new());
        }

        public Balance(Balance initial)
            : this()
        {
            foreach (var pair in initial._lookup)
            {
                _lookup.Add(pair);
            }
        }

        public ICollection<Account> Accounts => _lookup.Keys;

        public void Add(Account account, Amount amount)
        {
            _lookup[account] += amount;
        }

        public Amount this[Account account]
        {
            get => _lookup[account];
        }

        public Balance Under(Account parent)
        {
            var balance = new Balance();

            foreach (var entry in _lookup)
            {
                if (entry.Key.IsUnder(parent))
                {
                    balance._lookup.Add(entry);
                }
            }

            return balance;
        }
    }
}
