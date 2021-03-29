using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class FastPool
    {
        Dictionary<Type, object> pools = new Dictionary<Type, object>();

        public FastList<T> AllocateList<T>()
        {
            return new FastList<T>(this);
        }

        public FastStack<T> AllocateStack<T>()
        {
            return new FastStack<T>(this);
        }

        public void Free<T>(FastList<T> list)
        {
            list.Clear();
        }

        public T Allocate<T>()
        {
            var pool = pools.GetOrCreate(typeof(T), x => new Pool<T>()) as Pool<T>;
            return pool.Allocate();
        }

        public T[] AllocateArray<T>(int size)
        {
            var pool = pools.GetOrCreate(typeof(T), x => new Pool<T>()) as Pool<T>;
            return pool.AllocateArray(size);
        }

        public void Release<T>(T item)
        {
            var pool = pools.GetOrCreate(typeof(T), x => new Pool<T>()) as Pool<T>;
            pool.Release(item);
        }

        public void ReleaseArray<T>(T[] item)
        {
            var pool = pools.GetOrCreate(typeof(T), x => new Pool<T>()) as Pool<T>;
            pool.ReleaseArray(item);
        }


        class Pool<T>
        {
            private Queue<T> queue = new Queue<T>();

            internal T Allocate()
            {
                if (queue.TryDequeue(out var r))
                    return r;
                return Activator.CreateInstance<T>();
            }

            internal void Release(T item)
            {
                queue.Enqueue(item);
            }

            private Queue<T[]>[] Queues = new Queue<T[]>[] {
                new Queue<T[]>(),
                new Queue<T[]>(),
                new Queue<T[]>(),
                new Queue<T[]>()
            };

            private T[] AllocateInternal(int i)
            {
                if (Queues[i].TryDequeue(out var a))
                    return a;
                return new T[(i+1)*4];
            }

            private void ReleaseInternal(int i, T[] items)
            {
                Queues[i].Enqueue(items);
            }

            public T[] AllocateArray(int size)
            {
                if (size < 17) {
                    if (size < 13) {
                        if (size < 9) {
                            if (size < 4) {
                                return AllocateInternal(0);
                            }
                            return AllocateInternal(1);
                        }
                        return AllocateInternal(2);
                    }
                    return AllocateInternal(3);
                }
                return new T[size];
            }

            public void ReleaseArray(T[] item)
            {
                var size = item.Length;
                switch (size)
                {
                    case 16:
                        ReleaseInternal(4, item);
                        break;
                    case 12:
                        ReleaseInternal(3, item);
                        break;
                    case 8:
                        ReleaseInternal(2, item);
                        break;
                    case 4:
                        ReleaseInternal(0, item);
                        break;

                }
            }
        }

        private Queue<StringBuilder> fastStringBuilders = new Queue<StringBuilder>();

        internal FastStringBuilder AllocateStringBuilder()
        {
            if (fastStringBuilders.TryDequeue(out var sb))
                return new FastStringBuilder(this, sb);
            return new FastStringBuilder(this, new StringBuilder());
        }

        internal void Release(in FastStringBuilder sb)
        {
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
