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
        protected override CodeInfo VisitUnary(YUnaryExpression yUnaryExpression)
        {
            Visit(yUnaryExpression.Target);
            switch(yUnaryExpression.Operator)
            {
                case YUnaryOperator.Negative:
                    il.Emit(OpCodes.Neg);
                    return CodeInfo.HasStack;
                case YUnaryOperator.Not:
                    il.EmitConstant(0);
                    il.Emit(OpCodes.Ceq);
                    return CodeInfo.HasStack;
            }
            throw new NotImplementedException();
        }
    }
}
