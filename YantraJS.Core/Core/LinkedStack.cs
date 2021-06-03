using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{
    //public abstract class LinkedStackItem<T>: IDisposable
    //    where T: LinkedStackItem<T>
    //{
    //    internal T Parent;

    //    internal LinkedStack<T> stack;

    //    public virtual void Dispose()
    //    {
    //        stack._Top = Parent;
    //    }

        

    //    public void Pop()
    //    {
    //        stack._Top = Parent;
    //    }

    //    //internal virtual void Init()
    //    //{
    //    //}
    //}

    //public class LinkedStack<T>
    //    where T: LinkedStackItem<T>
    //{

    //    internal T _Top = null;

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public T Push(T item)
    //    {
    //        item.Parent = this._Top;
    //        this._Top = item;
    //        item.stack = this;
    //        // item.Init();
    //        return item;
    //    }

    //    public T Top {
    //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //        get => _Top;
    //    }

    //    public T Switch(T top)
    //    {
    //        var current = this._Top;
    //        this._Top = top;
    //        return current;
    //    }

    //}
}
