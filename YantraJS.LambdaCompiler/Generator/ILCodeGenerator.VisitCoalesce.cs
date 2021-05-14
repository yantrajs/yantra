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
            using (il.Branch())
            {
                il.Emit(OpCodes.Pop);

                // is it assign...
                Visit(yCoalesceExpression.Right);
            }
            il.MarkLabel(notNull);
            return true;
        }
    }
}
