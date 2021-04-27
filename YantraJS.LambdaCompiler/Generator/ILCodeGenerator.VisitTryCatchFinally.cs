using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitTryCatchFinally(YTryCatchFinallyExpression tryCatchFinallyExpression)
        {
            var tcb = new TryCatchBlock();
            tryCatchBlocks.Push(tcb);

            try
            {
                var @endTry = il.BeginExceptionBlock();

                tcb.EndTry = endTry;

                Visit(tryCatchFinallyExpression.Try);

                il.Emit(OpCodes.Leave, endTry);
                il.EndExceptionBlock();

                if (tryCatchFinallyExpression.Catch != null)
                {
                    il.BeginCatchBlock(typeof(Exception));
                    if(tryCatchFinallyExpression.CatchParameter == null)
                    {
                        il.Emit(OpCodes.Pop);
                    } else
                    {
                        var v = variables[tryCatchFinallyExpression.CatchParameter];
                        il.EmitSaveLocal(v.LocalBuilder.LocalIndex);
                    }

                    Visit(tryCatchFinallyExpression.Catch);
                    il.Emit(OpCodes.Leave, endTry);
                }

                if(tryCatchFinallyExpression.Finally != null)
                {
                    il.BeginFinallyBlock();
                    Visit(tryCatchFinallyExpression.Finally);
                }

                il.MarkLabel(endTry);

                

                return true;
            }
            finally
            {
                tryCatchBlocks.Pop();
            }
        }
    }
}
