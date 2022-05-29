#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {

        private CodeInfo VisitTailCall(YCallExpression callExpression)
        {
            var parameters = callExpression.Method.GetParameters();
            if (parameters.Any(p => p.IsOut))
                return false;

            if (callExpression.Target != null)
                Visit(callExpression.Target);

            EmitParameters(callExpression.Method, callExpression.Arguments, callExpression.Type);
            // .net tail call works only in single threaded mode
            // we are unable to run unit tests with following
            // uncommented, still looking for the answer !!
            // il.Emit(OpCodes.Tailcall);
            il.Emit(!callExpression.Method.IsStatic
                ? OpCodes.Callvirt
                : OpCodes.Call, callExpression.Method);
            il.Emit(OpCodes.Ret);
            return true;
        }


        protected override CodeInfo VisitCall(YCallExpression yCallExpression)
        {
            using (tempVariables.Push())
            {
                if(yCallExpression.Target != null)
                {
                    Visit(yCallExpression.Target);
                }

                var a = EmitParameters(yCallExpression.Method, yCallExpression.Arguments, yCallExpression.Type);
                il.EmitCall(yCallExpression.Method);
                a();
            }
            return true;
        }
    }
}
