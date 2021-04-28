using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Converters;

namespace YantraJS
{
    public static class LinqExtensions
    {

        public static T FastCompileWithoutNested<T>(this Expression<T> expression)
        {
            var ll = LinqConverters.ToLLExpression(expression);
            return ll.Compile<T>();
        }

        public static T FastCompile<T>(this Expression<T> expression)
        {
            var ll = LinqConverters.ToLLExpression(expression);
            return ll.CompileWithNestedLambdas<T>();
        }
    }
}
