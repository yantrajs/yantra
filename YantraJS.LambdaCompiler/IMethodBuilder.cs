using System.Collections.Generic;
using System.Linq.Expressions;

namespace YantraJS
{
    public interface IMethodBuilder
    {
        Expression Create(string name, LambdaExpression lambdaExpression);
    }
}
