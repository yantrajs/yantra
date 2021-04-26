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
        protected override CodeInfo VisitField(YFieldExpression yFieldExpression)
        {
            Visit(yFieldExpression.Target);

            if (RequiresAddress)
            {
                il.Emit(OpCodes.Ldflda, yFieldExpression.FieldInfo);
                return true;
            }

            il.Emit(OpCodes.Ldfld, yFieldExpression.FieldInfo);
            return true;
        }
    }
}
