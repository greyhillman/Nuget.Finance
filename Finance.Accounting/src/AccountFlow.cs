using System.Collections.Generic;

namespace Finance
{
    public class AccountFlow
    {
        private readonly DefaultDictionary<Account, DefaultDictionary<Account, Amount>> _forward;
        private readonly DefaultDictionary<Account, DefaultDictionary<Account, Amount>> _backward;

        public ICollection<Account> Sources => _forward.Keys;
        public ICollection<Account> Sinks => _backward.Keys;

        public AccountFlow()
        {
            _forward = new DefaultDictionary<Account, DefaultDictionary<Account, Amount>>(() =>
            {
                return new DefaultDictionary<Account, Amount>(() => new());
            });
            _backward = new DefaultDictionary<Account, DefaultDictionary<Account, Amount>>(() =>
            {
                return new DefaultDictionary<Account, Amount>(() => new());
            });
        }

        public void Add(Movement movement)
        {
            var from = movement.From;
            var to = movement.To;

            _forward[from][to] += movement.Amount;
            _backward[to][from] += movement.Amount;
        }

        public IEnumerable<Account> GetSinks(Account from)
        {
            return _forward[from].Keys;
        }

        public IEnumerable<Account> GetSources(Account to)
        {
            return _backward[to].Keys;
        }

        public Amount GetFlows(Account from, Account to)
        {
            return _forward[from][to];
        }
    }
}