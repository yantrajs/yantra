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
        protected override CodeInfo VisitConditional(YConditionalExpression yConditionalExpression)
        {
            // optimize for jumps...

            var trueEnd = il.DefineLabel();
            var falseBegin = il.DefineLabel();
            
            Visit(yConditionalExpression.test);
            
            il.Emit(OpCodes.Brfalse, 
                yConditionalExpression.@false != null 
                ? falseBegin
                : trueEnd);
            
            Visit(yConditionalExpression.@true);

            if(yConditionalExpression.@false != null)
            {
                il.Emit(OpCodes.Br, trueEnd);

                il.MarkLabel(falseBegin);
                Visit(yConditionalExpression.@false);
            }

            il.MarkLabel(trueEnd);
            return true;
        }
    }
}
