#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
       
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
            if(yCallExpression.Method.ReturnType != typeof(void))
            {
                return CodeInfo.HasStack;
            }
            return true;
        }
    }
}
