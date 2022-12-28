using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Finance
{
    public class AccountTree : IEnumerable<Account>
    {
        private readonly Dictionary<string, AccountTree> _children;

        public AccountTree()
        {
            _children = new();
        }

        public void Add(Account account)
        {
            Add(account.Branch);
        }

        private void Add(string[] account)
        {
            if (account.Length == 0)
            {
                return;
            }

            var childRoot = account[0];

            if (!_children.ContainsKey(childRoot))
            {
                _children[childRoot] = new AccountTree();
            }

            _children[childRoot].Add(account[1..]);
        }

        public IEnumerable<Account> GetUnder(Account parent)
        {
            var branch = parent.Branch;

            var tree = this;
            for (var depth = 0; depth < branch.Length; depth++)
            {
                var topAccount = branch[depth];
                if (tree._children.ContainsKey(topAccount))
                {
                    tree = tree._children[topAccount];
                    continue;
                }
                else
                {
                    tree = new AccountTree();
                    break;
                }
            }

            yield return parent;
            foreach (var child in tree)
            {
                var account = parent.Branch.Concat(child.Branch).ToArray();

                yield return new Account(account);
            }
        }

        public IEnumerator<Account> GetEnumerator()
        {
            var topAccounts = _children.Keys.OrderBy(x => x);

            foreach (var topAccount in topAccounts)
            {
                yield return new Account(topAccount);

                foreach (var child in _children[topAccount])
                {
                    var account = new[] { topAccount }.Concat(child.Branch).ToArray();

                    yield return new Account(account);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}