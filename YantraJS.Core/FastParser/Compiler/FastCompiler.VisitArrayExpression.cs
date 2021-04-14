using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitArrayExpression(AstArrayExpression arrayExpression)
        {
            var list = pool.AllocateList<Expression>(arrayExpression.Elements.Length);
            try {
                var e = arrayExpression.Elements.GetEnumerator();
                while(e.MoveNext(out var item))
                {
                    list.Add(item == null
                        ? Expression.Constant(null, typeof(JSValue))
                        : Visit(item));
                }
                return JSArrayBuilder.New(list);
            } finally {
                list.Clear();
            }
        }
    }
}
