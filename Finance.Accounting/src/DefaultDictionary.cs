using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Finance;

public class DefaultDictionary<K, V> : IDictionary<K, V> where K : notnull
{
    private readonly IDictionary<K, V> _base;
    private readonly Func<V> _gen;

    public DefaultDictionary(Func<V> gen)
    {
        _base = new Dictionary<K, V>();
        _gen = gen;
    }

    public V this[K key]
    {
        get
        {
            if (!_base.ContainsKey(key))
            {
                // It makes the dictionary easier to work with if we
                // set the newly generated value.
                this[key] = _gen();
            }

            return _base[key];
        }
        set
        {
            _base[key] = value;
        }
    }

    public ICollection<K> Keys => _base.Keys;

    public ICollection<V> Values => _base.Values;

    public int Count => _base.Count;

    public bool IsReadOnly => _base.IsReadOnly;

    public void Add(K key, V value)
    {
        _base.Add(key, value);
    }

    public void Add(KeyValuePair<K, V> item)
    {
        _base.Add(item);
    }

    public void Clear()
    {
        _base.Clear();
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
        return _base.Contains(item);
    }

    public bool ContainsKey(K key)
    {
        return _base.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
        _base.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        return _base.GetEnumerator();
    }

    public bool Remove(K key)
    {
        return _base.Remove(key);
    }

    public bool Remove(KeyValuePair<K, V> item)
    {
        return _base.Remove(item);
    }

    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
    {
        return _base.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _base.GetEnumerator();
    }
}