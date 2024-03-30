using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;

namespace Finance
{
    public class Balance
    {
        private readonly DefaultDictionary<Account, Amount> _lookup;

        public ICollection<Account> Accounts => _lookup.Keys;

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

        public void Add(Account account, Amount amount)
        {
            _lookup[account] += amount;

            if (account.Parent != account)
            {
                Add(account.Parent, amount);
            }
        }

        public Amount this[Account account]
        {
            get => _lookup[account];
        }

        public static Balance operator +(Balance left, Balance right)
        {
            var result = new Balance(left);

            foreach (var account in right.Accounts)
            {
                var amount = right[account];

                result.Add(account, amount);
            }

            return result;
        }
    }
}
