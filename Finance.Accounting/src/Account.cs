using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Finance
{
    public readonly struct Account : IEquatable<Account>, IComparable<Account>
    {
        private readonly string[] _branch = Array.Empty<string>();

        public Account(params string[] branch)
        {
            if (branch == null)
            {
                throw new ArgumentException();
            }
            _branch = branch;
        }

        public string[] Branch => _branch;

        public override string ToString()
        {
            return string.Join(":", _branch);
        }

        public bool IsUnder(Account parent)
        {
            if (parent.Branch.Length >= this.Branch.Length)
            {
                return false;
            }

            for (var i = 0; i < parent.Branch.Length; i++)
            {
                if (parent.Branch[i] != Branch[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(Account first, Account other)
        {
            return first.Equals(other);
        }

        public static bool operator !=(Account first, Account other)
        {
            return !first.Equals(other);
        }

        public bool Equals(Account other)
        {
            if (other._branch.Length != this._branch.Length)
            {
                return false;
            }

            for (var i = 0; i < _branch.Length; i++)
            {
                if (other.Branch[i] != Branch[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            foreach (var part in _branch)
            {
                hashCode ^= part.GetHashCode();
            }

            return hashCode;
        }

        public int CompareTo(Account other)
        {
            for (var i = 0; ; i++)
            {
                if (i < this.Branch.Length && i < other.Branch.Length)
                {
                    var order = string.Compare(this.Branch[i], other.Branch[i], StringComparison.Ordinal);
                    if (order != 0)
                    {
                        return order;
                    }
                }
                else if (i < this.Branch.Length)
                {
                    return 1;
                }
                else if (i < other.Branch.Length)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}