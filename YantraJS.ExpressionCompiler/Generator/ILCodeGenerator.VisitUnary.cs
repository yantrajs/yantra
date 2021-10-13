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
                    return true;
                case YUnaryOperator.Not:
                    il.EmitConstant(0);
                    il.Emit(OpCodes.Ceq);
                    return true;
                case YUnaryOperator.OnesComplement:
                    il.Emit(OpCodes.Not);
                    return true;
            }
            throw new NotImplementedException();
        }
    }
}
