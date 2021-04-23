#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class FastPool
    {

        public void Dispose()
        {
            pools.Clear();
        }

        public class Scope : DisposableList {
            private readonly FastPool pool;

            public Scope(FastPool pool)
            {
                this.pool = pool;
            }

            public FastList<T> AllocateList<T>(int size = 0)
            {
                var l = pool.AllocateList<T>(size);
                Register(l);
                return l;
            }

            public Scope NewScope() => pool.NewScope();

        }

        public Scope NewScope()
        {
            return new Scope(this);
        }


        readonly Dictionary<Type, object> pools = new Dictionary<Type, object>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastList<T> AllocateList<T>()
        {
            return new FastList<T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastList<T> AllocateList<T>(int size)
        {
            return new FastList<T>(this, size);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastStack<T> AllocateStack<T>()
        {
            return new FastStack<T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free<T>(FastList<T> list)
        {
            list.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] AllocateArray<T>(int size)
        {
            var pool = pools.GetOrCreate(typeof(T), x => new Pool<T>()) as Pool<T>;
            return pool!.AllocateArray(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseArray<T>(T[] item)
        {
            var pool = pools.GetOrCreate(typeof(T), x => new Pool<T>()) as Pool<T>;
            pool!.ReleaseArray(item);
        }


        class Pool<T>
        {
            private bool clear = false;

            public Pool(int buckets = 8)
            {
                clear = !typeof(T).IsValueType;

                Queues = new Queue<T[]>[buckets];
                for (int i = 0; i < buckets; i++)
                {
                    Queues[i] = new Queue<T[]>(8);
                }
                
            }

            private readonly Queue<T[]>[] Queues;

            private (int index, int size) MapSize(int size) {
                var begin = 4;
                int i;
                for(i = 0; i < Queues.Length ; i++)
                {
                    if(size <= begin)
                    {
                        break;
                    }
                    begin = begin << 1;
                }
                if (i < Queues.Length)
                {
                    return (i, begin);
                }
                return (-1, size);
            }

            private T[] AllocateInternal(int i)
            {
                var (index, size) = MapSize(i);
                if (index >= 0) {
                    if (Queues[index].TryDequeue(out var a))
                        return a;
                    // try one higher...
                    //if (index < Queues.Length - 1)
                    //{
                    //    if (Queues[index + 1].TryDequeue(out a))
                    //        return a;
                    //}
                }
                return new T[size];
            }

            private void ReleaseInternal(int i, T[] items)
            {
                ref var q = ref Queues[i];
                if (q.Count < 8)
                {
                    if(clear)
                        Array.Clear(items, 0, items.Length);
                    q.Enqueue(items);
                }
            }

            public T[] AllocateArray(int size)
            {
                return AllocateInternal(size);
            }

            public void ReleaseArray(T[] item)
            {
                var (index, size) = MapSize(item.Length);
                if(index >= 0) {
                    ReleaseInternal(index, item);
                }
            }
        }

        private readonly Queue<StringBuilder> fastStringBuilders = new Queue<StringBuilder>();

        internal FastStringBuilder AllocateStringBuilder()
        {
            if (fastStringBuilders.TryDequeue(out var sb))
                return new FastStringBuilder(this, sb);
            return new FastStringBuilder(this, new StringBuilder());
        }

        internal void Release(in FastStringBuilder sb)
        {
            if(fastStringBuilders.Count < 5)
                fastStringBuilders.Enqueue(sb.Builder);
        }
    }

    public readonly struct FastStringBuilder
    {
        public readonly StringBuilder Builder;
        private readonly FastPool pool;

        public FastStringBuilder(FastPool pool, StringBuilder sb)
        {
            this.pool = pool;
            this.Builder = sb;
            sb.Length = 0;
        }

        public override string ToString()
        {
            return Builder.ToString();
        }

        public void Clear()
        {
            pool.Release(in this);
        }
    }
}
