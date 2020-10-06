using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace WebAtoms.CoreJS.Core.Storage
{
    internal interface IBitTrie<TKey, TValue, TNode>
    {
        long Count { get; }
        void Save(TKey key, TValue value);

        TValue GetOrCreate(TKey key, Func<TValue> value);

        bool TryGetValue(TKey key, out TValue value);

        bool RemoveAt(TKey key);

        bool HasKey(TKey key);

        bool TryRemove(TKey key, out TValue value);

        // ref TNode GetTrieNode(TKey key, bool create = false);

        TValue this[TKey key] { get; set; }

        IEnumerable<(TKey Key, TValue Value)> AllValues { get; }

        int Update(Func<TKey, TValue, (bool replace, TValue value)> func);

        // IEnumerable<(TKey key, TValue value)> Enumerate(TKey start = default);

    }
}
