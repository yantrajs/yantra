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
            public Pool()
            {
                clear = !typeof(T).IsValueType;
            }

            private readonly Queue<T[]>[] Queues = new Queue<T[]>[] {
                new Queue<T[]>(8),
                new Queue<T[]>(8),
                new Queue<T[]>(8),
                new Queue<T[]>(8)
            };

            private (int index, int size) MapSize(int size) {
                if (size <= 16) {
                    if (size <= 12) {
                        if (size <= 8) {
                            if (size <= 4) {
                                return (0, 4);
                            }
                            return (1, 8);
                        }
                        return (2, 12);
                    }
                    return (3, 16);
                }

                // change size to multiple of 8 to avoid single allocation..
                size = ((size >> 2) + 1) << 2;

                return (-1, size);
            }

            private T[] AllocateInternal(int i)
            {
                var (index, size) = MapSize(i);
                if (index >= 0) {
                    if (Queues[index].TryDequeue(out var a))
                        return a;
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
