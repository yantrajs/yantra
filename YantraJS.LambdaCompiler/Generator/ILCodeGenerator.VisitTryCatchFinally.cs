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
            var tcb = il.BeginTry();

            try
            {

                Visit(tryCatchFinallyExpression.Try);


                if (tryCatchFinallyExpression.Catch != null)
                {
                    tcb.BeginCatch(typeof(Exception));
                    if(tryCatchFinallyExpression.Catch.Parameter == null)
                    {
                        il.Emit(OpCodes.Pop);
                    } else
                    {
                        var v = variables[tryCatchFinallyExpression.Catch.Parameter];
                        il.EmitSaveLocal(v.LocalBuilder.LocalIndex);
                    }

                    Visit(tryCatchFinallyExpression.Catch.Body);
                }

                if(tryCatchFinallyExpression.Finally != null)
                {
                    tcb.BeginFinally();
                    Visit(tryCatchFinallyExpression.Finally);
                }

                return true;
            }
            finally
            {
                tcb.Dispose();
            }
        }
    }
}
