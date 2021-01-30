using System;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.LinqExpressions.Generators
{
    //public class FastStack<T>
    //{
    //    private int _count;
    //    public int Count => _count;

    //    private FastStackNode Start = null;

    //    class FastStackNode {
    //        internal T[] Nodes = new T[8];
    //        internal FastStackNode Parent;
    //    }

    //    public void Push(T item)
    //    {
    //        var index = _count % 8;
    //        if (index == 0)
    //        {
    //            Start = new FastStackNode { Parent = Start };
    //        }
    //        Start.Nodes[index] = item;
    //        _count++;
    //    }

    //    public T Pop()
    //    {
    //        if (_count == 0)
    //            throw new KeyNotFoundException();
    //        _count--;
    //        var index = _count % 8;
    //        ref var t = ref Start.Nodes[index];
    //        if (index == 0)
    //        {
    //            Start = Start.Parent ?? Start;
    //        }
    //        return t;
    //    }

    //    public T Peek()
    //    {
    //        if (_count == 0)
    //            throw new KeyNotFoundException();
    //        return Start.Nodes[(_count - 1) % 8];
    //    }

    //}

    public struct InstructionStack
    {
        public static object StackItem = new object();

        private (uint label, Func<object> instruction)[] items;
        private int index;

        public int Count => index + 1;

        public InstructionStack(int size)
        {
            items = new (uint label, Func<object> instruction)[size];
            this.index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int size)
        {
            if (items.Length <= size)
            {
                Array.Resize(ref items, ((size >> 2)+1) << 2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (uint label, Func<object> instruction) Pop()
        {
            var item = items[index];
            items[index].instruction = null;
            index--;
            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (uint label, Func<object> instruction) Peek()
        {
            return items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Push(uint label)
        {
            EnsureCapacity(index + 1);
            items[++index] = (label, null);
            return StackItem;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Push(uint label, Func<object> instruction)
        {
            EnsureCapacity(index + 1);
            items[++index] = (label, instruction);
            return StackItem;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Push(Func<object> instruction)
        {
            EnsureCapacity(index + 1);
            items[++index] = (0, instruction);
            return StackItem;
        }

        internal void Clear()
        {
            while (index >= 0)
            {
                items[index].instruction = null;
                index--;
            }
        }
    }
}
