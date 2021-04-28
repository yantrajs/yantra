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
        protected override CodeInfo VisitIndex(YIndexExpression yIndexExpression)
        {
            Visit(yIndexExpression.Target);
            EmitParameters(yIndexExpression.GetMethod, yIndexExpression.Arguments, yIndexExpression.Type);
            il.EmitCall(yIndexExpression.GetMethod);
            if (RequiresAddress)
            {
                var temp = this.tempVariables[yIndexExpression.Property.PropertyType];
                il.EmitSaveLocal(temp.LocalIndex);
                il.EmitLoadLocalAddress(temp.LocalIndex);
            }
            return true;
        }
    }
}
