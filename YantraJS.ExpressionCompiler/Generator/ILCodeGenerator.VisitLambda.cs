using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitLambda(YLambdaExpression yLambdaExpression)
        {
            // check if it is a relay...
            if (yLambdaExpression.IsRelay())
            {
                // get all internally passed parameters...
                var captures = yLambdaExpression.GetRelayCaptures().Select(x => x as YExpression).AsSequence();
                return Visit(methodBuilder.Relay(captures, yLambdaExpression));
            }
            return Visit(methodBuilder.Relay(Sequence<YExpression>.Empty, yLambdaExpression));
        }
    }
}
