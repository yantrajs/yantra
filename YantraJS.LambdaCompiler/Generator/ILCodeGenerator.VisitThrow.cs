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
        protected override CodeInfo VisitThrow(YThrowExpression throwExpression)
        {
            Visit(throwExpression.Expression);
            il.Emit(OpCodes.Throw);
            // leaving one item on stack so 
            // block code generator can continue
            // il.Emit(OpCodes.Ldnull);
            return true;
        }
    }
}
