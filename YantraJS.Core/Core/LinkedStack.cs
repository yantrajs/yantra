using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    public abstract class LinkedStackItem<T>: IDisposable
        where T: LinkedStackItem<T>
    {
        public T Parent { get; internal set; }

        internal LinkedStack<T> stack;

        public virtual void Dispose()
        {
            stack._Top = Parent;
        }

        internal virtual void Init()
        {
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
            item.Init();
            return item;
        }

        public T Top => _Top;

        public T Switch(T top)
        {
            var current = this._Top;
            this._Top = top;
            return current;
        }

    }
}
