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
        protected override CodeInfo VisitCoalesce(YCoalesceExpression yCoalesceExpression)
        {
            var notNull = il.DefineLabel("coalesce");
            Visit(yCoalesceExpression.Left);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Brtrue, notNull);
            il.Emit(OpCodes.Pop);
            Visit(yCoalesceExpression.Right);
            il.MarkLabel(notNull);
            return true;
        }
    }
}
