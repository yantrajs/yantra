using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Converters
{

    public partial class LinqConverter
    {
        protected YExpression[] Visit(IEnumerable<Expression> list)
        {
            return list.Select(Visit).ToArray();
        }

        protected override YExpression VisitCall(MethodCallExpression node)
        {
            var target = Visit(node.Object);
            var list = node.Arguments.Select(a => Visit(a)).ToArray();
            return YExpression.Call(target, node.Method, list);
        }


    }
}
