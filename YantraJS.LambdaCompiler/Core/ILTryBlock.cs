using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using YantraJS.Generator;

namespace YantraJS.Core
{
    public class ILTryBlock : LinkedStackItem<ILTryBlock>
    {
        private bool isCatch = false;
        private bool isFinally = false;

        private ILWriter il;
        private readonly ILWriterLabel label;

        private List<(ILWriterLabel hop, ILWriterLabel final, int localIndex)> pendingJumps 
            = new List<(ILWriterLabel,ILWriterLabel, int)>();



        public ILTryBlock(ILWriter iLWriter, Label label)
        {
            this.il = iLWriter;
            this.label = new ILWriterLabel(label, null);
        }

        public void BeginCatch(Type type)
        {
            if (isFinally)
                throw new InvalidOperationException($"Cannot start catch after finally has begin");
            isCatch = true;
            il.Emit(OpCodes.Leave, label);

            il.BeginCatchBlock(type);            
        }

        public void BeginFinally()
        {
            if (isFinally)
                throw new InvalidOperationException($"You already in the finally block");
            isFinally = true;
            il.Emit(OpCodes.Leave, label);
        }

        public override void Dispose()
        {
            if(isCatch)
            {
                il.Emit(OpCodes.Leave, label);
            }

            if (!(isCatch || isFinally))
                throw new InvalidOperationException($"Cannot finish try block without catch/finally");

            base.Dispose();

            // jump all pending
            il.EndExceptionBlock();

            foreach(var (hop,jump, index) in pendingJumps)
            {
                il.MarkLabel(hop);
                if (index > 0)
                {
                    il.EmitLoadLocal(index);
                }
                il.Branch(jump);
            }

            il.MarkLabel(label);
        }

        internal void Branch(ILWriterLabel label, int index = 0)
        {
            if(label.TryBlock == this)
            {
                il.Goto(label, index);
                return;
            }
            pendingJumps.Add((il.DefineLabel(), label, index));
        }
    }
}
