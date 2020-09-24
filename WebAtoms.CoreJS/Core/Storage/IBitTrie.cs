using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace WebAtoms.CoreJS.Core.Storage
{
    internal interface IBitTrie<TKey, TValue>
    {

        void Save(TKey key, TValue value);

        TValue GetOrCreate(TKey key, TValue value);

    }
}
