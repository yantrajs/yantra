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
        protected override CodeInfo VisitLambda(YLambdaExpression yLambdaExpression)
        {
            throw new NotSupportedException($"Nested lambda are not supported, use CompileWithNestedLambda or CompileToMethod");
        }
    }
}
