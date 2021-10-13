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
        protected override CodeInfo VisitListInit(YListInitExpression node)
        {
            Visit(node.NewExpression);

            foreach (var ei in node.Members)
            {
                il.Emit(OpCodes.Dup);
                foreach(var p in ei.Arguments)
                {
                    Visit(p);
                }
                il.EmitCall(ei.AddMethod);
            }

            return true;
        }
    }
}
