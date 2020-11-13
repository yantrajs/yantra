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

        public (uint Key, string Name) Get(string name)
        {
            uint r = map.GetOrCreate(name, (n) => ((uint)Interlocked.Increment(ref id)));
            return (r, name);
        }

        public bool TryGetValue(string name, out (uint Key, string Name) value)
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

    public struct ConcurrentStringMap<T>
    {

        private StringMap<T> Map;

        private ReaderWriterLockSlim lockSlim;

        public static ConcurrentStringMap<T> Create()
        {
            return new ConcurrentStringMap<T> {
                lockSlim = new ReaderWriterLockSlim()
            };
        }


        public T this[string key]
        {
            get
            {
                
                lockSlim.EnterReadLock();
                var value = Map[key];
                lockSlim.ExitReadLock();
                return value;
            }
            set
            {
                lockSlim.EnterWriteLock();
                Map[key] = value;
                lockSlim.ExitWriteLock();
            }
        }

        public bool TryGetValue(string key, out T value)
        {
            lockSlim.EnterReadLock();
            var r = Map.TryGetValue(key, out value);
            lockSlim.ExitReadLock();
            return r;
        }

        internal T GetOrCreate(string key, Func<string,T> value)
        {
            try
            {
                lockSlim.EnterUpgradeableReadLock();
                if (Map.TryGetValue(key, out var v))
                    return v;
                var r = value(key);
                lockSlim.EnterWriteLock();
                try
                {
                    Map[key] = r;
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
    }


    public struct ConcurrentUInt32Map<T>
    {

        private UInt32Map<T> Map;

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
                var value = Map[key];
                lockSlim.ExitReadLock();
                return value;
            }
            set
            {
                lockSlim.EnterWriteLock();
                Map[key] = value;
                lockSlim.ExitWriteLock();
            }
        }

        public bool TryGetValue(uint key, out T value)
        {
            lockSlim.EnterReadLock();
            var r = Map.TryGetValue(key, out value);
            lockSlim.ExitReadLock();
            return r;
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
                    Map[key] = r;
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
    }
}
