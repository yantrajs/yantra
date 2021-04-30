using System.Collections.Generic;
using YantraJS.Expressions;

namespace YantraJS
{
    public interface IMethodBuilder
    {
        YExpression Relay(YExpression[] closures, YLambdaExpression innerLambda);
    }
}
