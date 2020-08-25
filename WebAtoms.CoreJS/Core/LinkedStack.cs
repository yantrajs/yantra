using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{

    public class LinkedStack<T>
        where T:class
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

        private StackNode _Top { get; set; } = null;

        public StackNode Push(T item)
        {
            return new StackNode(this, item);
        }

        public T Top => _Top?.Value;

        public class StackNode : IDisposable
        {
            private readonly LinkedStack<T> owner;
            private StackNode last;

            public T Value { get; }

            public StackNode(LinkedStack<T> owner, T value)
            {
                this.owner = owner;
                this.last = owner._Top;
                owner._Top = this;
                this.Value = value;
            }

            public void Dispose()
            {
                owner._Top = last;
            }
        }

    }
}
