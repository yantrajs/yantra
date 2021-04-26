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

        private void EmitParameters(MethodBase method, YExpression[] args)
        {
            using (tempVariables.Push())
            {
                var pa = method.GetParameters();
                for (int i = 0; i < pa.Length; i++)
                {

                    var p = pa[i];

                    if (i < args.Length)
                    {
                        using (addressScope.Push(p))
                        {
                            Visit(args[i]);
                        }
                        continue;
                    }
                    if (!p.HasDefaultValue)
                        throw new ArgumentException($"Not enough arguments to create object");
                    il.EmitConstant(p.RawDefaultValue);
                }
            }

        }

        protected override CodeInfo VisitCall(YCallExpression yCallExpression)
        {
            EmitParameters(yCallExpression.Method, yCallExpression.Arguments);
            il.EmitCall(yCallExpression.Method);
            return true;
        }
    }
}
