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

        private Action EmitParameters(MethodBase method, YExpression[] args, Type returnType)
        {
            List<(int temp, YExpression exp)>? saveList = null;

            var pa = method.GetParameters();
            for (int i = 0; i < pa.Length; i++)
            {

                var p = pa[i];

                if (i < args.Length)
                {
                    var a = args[i];

                    if(p.IsOut || p.ParameterType.IsByRef)
                    {
                        if(a.NodeType != YExpressionType.Parameter)
                        {
                            var temp = tempVariables[p.ParameterType];
                            saveList ??= new List<(int temp, YExpression exp)>();
                            saveList.Add((temp.LocalIndex, a));

                            Visit(a);
                            // save in temp...
                            il.EmitSaveLocal(temp.LocalIndex);

                            il.EmitLoadLocalAddress(temp.LocalIndex);
                            continue;
                        }
                    }

                    using (addressScope.Push(p))
                    {
                        Visit(a);
                    }
                    continue;
                }
                if (!p.HasDefaultValue)
                    throw new ArgumentException($"Not enough arguments to create object");
                il.EmitConstant(p.RawDefaultValue);
            }

            return Save;

            void Save()
            {
                if (saveList == null)
                    return;
                var rtIndex = 0;

                if(returnType != typeof(void))
                {
                    var t = tempVariables[returnType];
                    rtIndex = t.LocalIndex;

                    il.EmitSaveLocal(rtIndex);
                }

                foreach(var (temp,exp) in saveList)
                {
                    il.EmitLoadLocal(temp);
                    Assign(exp);
                }

                if(rtIndex != 0)
                {
                    il.EmitLoadLocal(rtIndex);
                }
            }
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
