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
            using (tempVariables.Push())
            {
                var tcb = il.BeginTry();

                // visit labels...
                tcb.CollectLabels(tryCatchFinallyExpression, labels);

                var hasType = tryCatchFinallyExpression.Try.Type != typeof(void);

                var result = hasType ? tempVariables[tryCatchFinallyExpression.Try.Type] : null;

                tcb.SavedLocal = hasType ? result.LocalIndex : -1;

                // we need to save this in local variable...
                Visit(tryCatchFinallyExpression.Try);
                if (hasType)
                {
                    il.EmitSaveLocal(result.LocalIndex);
                }



                if (tryCatchFinallyExpression.Catch != null)
                {
                    tcb.BeginCatch(typeof(Exception));
                    if (tryCatchFinallyExpression.Catch.Parameter == null)
                    {
                        il.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        var v = variables[tryCatchFinallyExpression.Catch.Parameter];
                        il.EmitSaveLocal(v.LocalBuilder.LocalIndex);
                    }

                    Visit(tryCatchFinallyExpression.Catch.Body);
                    if (hasType)
                    {
                        il.EmitSaveLocal(result.LocalIndex);
                    }
                }

                if (tryCatchFinallyExpression.Finally != null)
                {
                    tcb.BeginFinally();
                    Visit(tryCatchFinallyExpression.Finally);
                }
                tcb.Dispose();
            }
            return true;
        }
    }
}
