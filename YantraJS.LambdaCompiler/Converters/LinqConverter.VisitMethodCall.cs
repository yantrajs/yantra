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

        private YExpression VisitCall(MethodCallExpression methodCallExpression)
        {
            var target = Visit(methodCallExpression.Object);
            var list = methodCallExpression.Arguments.Select(a => Visit(a)).ToArray();
            return YExpression.Call(target, methodCallExpression.Method, list);
        }


    }
}
