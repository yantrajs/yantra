using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS
{
    public interface IMethodBuilder
    {
        YExpression Relay(IFastEnumerable<YExpression> closures, YLambdaExpression innerLambda);
    }
}
