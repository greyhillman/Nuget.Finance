using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Finance.Accounting;

public class AccountTree
{
    private readonly string[] _prefix;
    private readonly SortedDictionary<string, AccountTree> _children;

    public AccountTree()
    {
        _prefix = [];
        _children = new();
    }

    private AccountTree(string[] prefix, string child)
    {
        _prefix = [.. prefix, child];
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
            _children[childRoot] = new AccountTree(_prefix, childRoot);
        }

        _children[childRoot].Add(account[1..]);
    }

    public AccountTree GetUnder(Account parent)
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

        return tree;
    }

    public IEnumerable<Account> EnumerateDown()
    {
        var topAccounts = _children.Keys;

        foreach (var topAccount in topAccounts)
        {
            yield return new Account([.. _prefix, topAccount]);

            foreach (var child in _children[topAccount].EnumerateDown())
            {
                yield return child;
            }
        }
    }

    public IEnumerable<Account> EnumerateUp()
    {
        var topAccounts = _children.Keys;

        foreach (var topAccount in topAccounts)
        {
            foreach (var child in _children[topAccount].EnumerateUp())
            {
                yield return child;
            }

            yield return new Account([.. _prefix, topAccount]);
        }
    }
}