using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public abstract class LinkedStackItem<T>: IDisposable
        where T: LinkedStackItem<T>
    {
        public T Parent { get; internal set; }

        internal LinkedStack<T> stack;

        public void Dispose()
        {
            stack._Top = Parent;
        }
    }

    public class LinkedStack<T>
        where T: LinkedStackItem<T>
    {

        internal T _Top { get; set; } = null;

        public T Push(T item)
        {
            item.Parent = this._Top;
            this._Top = item;
            item.stack = this;
            return item;
        }

        public T Top => _Top;

    }
}
