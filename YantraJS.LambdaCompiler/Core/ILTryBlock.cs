using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace YantraJS.Core
{
    public class ILTryBlock : LinkedStackItem<ILTryBlock>
    {
        private bool isTry = true;
        private bool isCatch = false;
        private bool isFinally = false;

        private ILWriter il;
        private readonly ILWriterLabel label;
        private List<ILWriterLabel> currentLabels = new List<ILWriterLabel>();

        private List<(ILWriterLabel hop, ILWriterLabel final)> pendingJumps 
            = new List<(ILWriterLabel,ILWriterLabel)>();



        public ILTryBlock(ILWriter iLWriter, Label label)
        {
            this.il = iLWriter;
            this.label = new ILWriterLabel(label);
        }

        internal void Register(ILWriterLabel newLabel)
        {
            currentLabels.Add(newLabel);
        }

        public void BeginCatch(Type type)
        {
            if (isFinally)
                throw new InvalidOperationException($"Cannot start catch after finally has begin");
            isCatch = true;
            isTry = false;
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

            base.Dispose();

            // jump all pending
            il.EndExceptionBlock();

            foreach(var (hop,jump) in pendingJumps)
            {
                il.MarkLabel(hop);
                il.Branch(jump);
            }

            il.MarkLabel(label);
        }

        internal void Branch(ILWriterLabel label)
        {
            pendingJumps.Add((il.DefineLabel(), label));
        }
    }
}
