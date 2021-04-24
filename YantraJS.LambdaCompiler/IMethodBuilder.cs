using System.Collections.Generic;
using YantraJS.Expressions;

namespace YantraJS
{
    public interface IMethodBuilder
    {
        YExpression Create(string name, YLambdaExpression lambdaExpression);
    }
}
