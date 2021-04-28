using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Generator
{
    public class TryCatchBlock: LinkedStackItem<TryCatchBlock>
    {
        public ILWriterLabel EndTry { get; internal set; }
        internal void Push(Action action)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ILCodeGenerator
    {

        private LinkedStack<TryCatchBlock> tryCatchBlocks
            = new LinkedStack<TryCatchBlock>();

        private void Goto(ILWriterLabel label)
        {
            il.Emit(OpCodes.Br, label);
        }

        private void PushBranch(Action action)
        {
            if(tryCatchBlocks.Top == null)
            {
                action();
                return;
            }

            tryCatchBlocks.Top.Push(action);
        }

    }
}
