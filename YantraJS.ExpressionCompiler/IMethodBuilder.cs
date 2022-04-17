using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS
{
    public interface IMethodBuilder
    {
        YExpression Relay(YExpression @this, IFastEnumerable<YExpression> closures, YLambdaExpression innerLambda);
    }
}
