using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Generator
{
    public class TryCatchBlock
    {
        public ILWriterLabel EndTry { get; internal set; }
        internal void Push(Action action)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ILCodeGenerator
    {

        private Stack<TryCatchBlock> tryCatchBlocks = new Stack<TryCatchBlock>();

        private void Goto(ILWriterLabel label)
        {
            il.Emit(OpCodes.Br, label);
        }

        private void PushBranch(Action action)
        {
            if(tryCatchBlocks.Count == 0)
            {
                action();
                return;
            }

            tryCatchBlocks.Peek().Push(action);
        }

    }
}
