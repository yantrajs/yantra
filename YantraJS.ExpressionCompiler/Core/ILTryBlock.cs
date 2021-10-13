using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS.Core
{
    public class ILTryBlock : LinkedStackItem<ILTryBlock>
    {
        private bool isCatch = false;
        private bool isFinally = false;

        internal readonly ILWriter il;
        private readonly ILWriterLabel label;

        private Sequence<(ILWriterLabel hop, ILWriterLabel final, int localIndex)> pendingJumps 
            = new Sequence<(ILWriterLabel,ILWriterLabel, int)>();
        
        internal int SavedLocal;

        public ILTryBlock(ILWriter iLWriter, Label label)
        {
            this.il = iLWriter;
            // this.label = new ILWriterLabel(label, null);
            this.label = iLWriter.DefineLabel("tryEnd");
        }

        internal void CollectLabels(YTryCatchFinallyExpression exp, LabelInfo labels)
        {
            TryCatchLabelMarker.Collect(exp, this, labels);
        }

        public void BeginCatch(Type type)
        {
            if (isFinally)
                throw new InvalidOperationException($"Cannot start catch after finally has begin");
            isCatch = true;

            // il.EmitConsoleWriteLine("Begin Catch");

            il.Emit(OpCodes.Leave, label);

            il.BeginCatchBlock(type);
        }

        public void BeginFinally()
        {
            if (isFinally)
                throw new InvalidOperationException($"You already in the finally block");
            isFinally = true;
            isCatch = false;
            il.Emit(OpCodes.Leave, label);

            il.BeginFinallyBlock();
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
            // il.ClearStack();

            foreach (var (hop,jump, index) in pendingJumps)
            {
                il.MarkLabel(hop);
                il.Branch(jump, index);
            }
            il.MarkLabel(label);
            if (SavedLocal >= 0)
            {
                il.EmitLoadLocal(SavedLocal);
            }

        }

        internal void Branch(ILWriterLabel label, int index = -1)
        {
            if(label.TryBlock == this)
            {
                il.Goto(label, index);
                return;
            }

            var hop = il.DefineLabel($"hop for {label.ID}");

            pendingJumps.Add((hop, label, index));
            il.Emit(OpCodes.Leave, hop);
        }
    }
}
