using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public class Block
    {

        public List<Expression> Steps = new List<Expression>();

        public void Add(Expression exp) => Steps.Add(exp);

        public Expression ToExpression()
        {
            Expression body;

            Steps = Steps.Where(x => x != null).ToList();

            switch (Steps.Count)
            {
                case 0:
                    return null;
                case 1:
                    body = Steps[0];
                    if (body.Type == typeof(Func<object>))
                        return body;
                    if (body.Type == typeof(void))
                    {
                        body = Expression.Block(body, Expression.Constant(null, typeof(object)));
                    }
                    break;
                default:
                    if (Steps.Last().Type == typeof(void))
                    {
                        Steps.Add(Expression.Constant(null, typeof(object)));
                        body = Expression.Block(Steps);
                    }
                    else
                    {
                        body = Expression.Block(Steps);
                    }
                    break;
            }
            if (body.Type == typeof(Func<object>))
                return body;
            return Expression.Lambda(body.AsObject());
        }

    }
}
