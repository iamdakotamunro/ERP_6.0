using System;
using System.Collections.Generic;

namespace ERP.Cache.Interface
{
    internal interface ICaching<T>
    {
        IList<T> ToList();
        void Remove();
    }

    internal interface ICaching<TKey,TValue>
    {
        TValue Get(TKey key);

        void Remove(TKey key);

        void Set(TKey key, TValue value);

        IList<TKey> GetAllKeys();
    }
}
