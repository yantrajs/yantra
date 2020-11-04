using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace YantraJS
{
    //public class CallStack
    //{

    //    static AsyncLocal<CallStack> _current = new AsyncLocal<CallStack>();

    //    public static CallStack Current => (_current.Value ?? (_current.Value = new CallStack()));

    //    Stack<INode> stack = new Stack<INode>();

    //    public IDisposable Push(INode node)
    //    {
    //        stack.Push(node);
    //        return new DisposableAction(() => stack.Pop());
    //    }


    //    public struct DisposableAction : IDisposable
    //    {

    //        readonly Action action;
    //        public DisposableAction(Action action)
    //        {
    //            this.action = action;
    //        }

    //        public void Dispose()
    //        {
    //            action();
    //        }
    //    }
    //}
}
