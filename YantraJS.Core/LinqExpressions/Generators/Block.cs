using System.Collections.Generic;
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
            switch (Steps.Count)
            {
                case 0:
                    body = Expression.Constant(null, typeof(object));
                    break;
                case 1:
                    body = Steps[0];
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
            return Expression.Lambda(body);
        }

    }
}
