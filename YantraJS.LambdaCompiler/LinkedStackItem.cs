using System;

namespace YantraJS
{
    public abstract class LinkedStackItem<T> : IDisposable
    where T : LinkedStackItem<T>
    {
        internal T Parent;

        internal LinkedStack<T> stack;

        public virtual void Dispose()
        {
            stack._Top = Parent;
        }



        public void Pop()
        {
            stack._Top = Parent;
        }

        //internal virtual void Init()
        //{
        //}
    }
}
