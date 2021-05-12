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
        protected override CodeInfo VisitConvert(YConvertExpression convertExpression)
        {
            Visit(convertExpression.Target);
            il.EmitCall(convertExpression.Method);
            return true;
        }
    }
}
