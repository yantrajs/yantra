using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{

    public class StackNode<T> : IDisposable
    {
        private readonly LinkedStack<T> owner;
        private StackNode<T> last;

        public T Value { get; }

        public StackNode(LinkedStack<T> owner, T value)
        {
            this.owner = owner;
            this.last = owner.Top;
            owner.Top = this;
            this.Value = value;
        }

        public void Dispose()
        {
            owner.Top = last;
        }
    }

    public class LinkedStack<T>
    {

        readonly Func<T> factory;
        public LinkedStack()
        {
            factory = null;
        }

        public LinkedStack(Func<T> factory)
        {
            this.factory = factory;
        }

        public StackNode<T> Top { get; internal set; } = null;

        public StackNode<T> PushNew()
        {
            return new StackNode<T>(this, factory());
        }
        
        public StackNode<T> Push(T item)
        {
            return new StackNode<T>(this, item);
        }

    }
}
