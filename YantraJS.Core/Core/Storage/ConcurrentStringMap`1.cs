using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace YantraJS.Core.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    public class ConcurrentNameMap
    {
        private ConcurrentStringMap<uint> map;
        private long id;

        public ConcurrentNameMap()
        {
            map = ConcurrentStringMap<uint>.Create();
            id = 0;
        }

        public (uint Key, StringSpan Name) Get(in StringSpan name)
        {
            uint r = map.GetOrCreate(name, (n) => ((uint)Interlocked.Increment(ref id)));
            return (r, name);
        }

        public bool TryGetValue(in StringSpan name, out (uint Key, StringSpan Name) value)
        {
            if(map.TryGetValue(name, out var i))
            {
                value = (i, name);
                return true;
            }
            value = (0, null);
            return false;
        }
    }

    public class ConcurrentStringMap<T>
    {

        private StringMap<T> Map;

        private ReaderWriterLockSlim lockSlim;

        public static ConcurrentStringMap<T> Create()
        {
            return new ConcurrentStringMap<T> {
                lockSlim = new ReaderWriterLockSlim()
            };
        }


        public T this[in StringSpan key]
        {
            get
            {
                
                lockSlim.EnterReadLock();
                try
                {
                    var value = Map[key];
                    return value;
                } finally
                {
                    lockSlim.ExitReadLock();
                }
            }
            set
            {
                lockSlim.EnterWriteLock();
                try
                {
                    Map.Put(key) = value;
                }
                finally
                {
                    lockSlim.ExitWriteLock();
                }
            }
        }

        public bool TryGetValue(in StringSpan key, out T value)
        {
            lockSlim.EnterReadLock();
            try
            {
                return Map.TryGetValue(key, out value);
            }
            finally
            {
                lockSlim.ExitReadLock();
            }
        }

        public T GetOrCreate(in StringSpan key, Func<StringSpan, T> value)
        {
            lockSlim.EnterUpgradeableReadLock();
            if (Map.TryGetValue(key, out var v))
            {
                lockSlim.ExitUpgradeableReadLock();
                return v;
            }
            T r;
            try
            {
                lockSlim.EnterWriteLock();
                r = value(key);
                Map.Put(key) = r;
            }
            finally
            {
                lockSlim.ExitWriteLock();
                lockSlim.ExitUpgradeableReadLock();
            }
            return r;
        }
    }


    public class ConcurrentUInt32Map<T>
    {

        private Uint32Map<T> Map;

        private ReaderWriterLockSlim lockSlim;

        public static ConcurrentUInt32Map<T> Create()
        {
            return new ConcurrentUInt32Map<T>
            {
                lockSlim = new ReaderWriterLockSlim()
            };
        }


        public T this[uint key]
        {
            get
            {

                lockSlim.EnterReadLock();
                try
                {
                    Map.TryGetValue(key, out var value);
                    return value;
                } finally
                {
                    lockSlim.ExitReadLock();
                }
            }
            set
            {
                lockSlim.EnterWriteLock();
                try
                {
                    Map.Put(key) = value;
                } finally {
                    lockSlim.ExitWriteLock();
                }
            }
        }

        public bool TryGetValue(uint key, out T value)
        {
            lockSlim.EnterReadLock();
            try
            {
                return Map.TryGetValue(key, out value);
            } finally
            {
                lockSlim.ExitReadLock();
            }
        }

        internal T GetOrCreate<TP>(uint key, Func<TP, T> value, in TP p)
        {
            try
            {
                lockSlim.EnterUpgradeableReadLock();
                if (Map.TryGetValue(key, out var v))
                    return v;
                var r = value(p);
                lockSlim.EnterWriteLock();
                try
                {
                    Map.Put(key) = r;
                }
                finally
                {
                    lockSlim.ExitWriteLock();
                }
                return r;
            }
            finally
            {
                lockSlim.ExitUpgradeableReadLock();
            }
        }

        internal T GetOrCreate(uint key, Func<T> value)
        {
            try
            {
                lockSlim.EnterUpgradeableReadLock();
                if (Map.TryGetValue(key, out var v))
                    return v;
                var r = value();
                lockSlim.EnterWriteLock();
                try
                {
                    Map.Put(key) = r;
                }
                finally
                {
                    lockSlim.ExitWriteLock();
                }
                return r;
            }
            finally
            {
                lockSlim.ExitUpgradeableReadLock();
            }
        }

        public IEnumerable<T> All
        {
            get
            {
                try {
                    lockSlim.EnterReadLock();
                    foreach (var k in Map.AllValues()) {
                        yield return k.Value;
                    }
                } finally {
                    lockSlim.ExitReadLock();
                }
            }
        }
    }
}
