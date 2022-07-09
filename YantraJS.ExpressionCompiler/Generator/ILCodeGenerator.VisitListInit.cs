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

            var me = node.Members.GetFastEnumerator();
            while(me.MoveNext(out var ei))
            {
                il.Emit(OpCodes.Dup);
                var ae = ei.Arguments.GetFastEnumerator();
                while(ae.MoveNext(out var p))
                {
                    Visit(p);
                }
                il.EmitCall(ei.AddMethod);
            }

            return true;
        }
    }
}
