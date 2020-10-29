using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core.Generator;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSAsyncFunctionBuilder
    {
        private static Type type = typeof(JSAsyncFunction);

        private static ConstructorInfo _New =
            type.Constructor(typeof(JSAsyncDelegate), typeof(string), typeof(string));

        public static Expression New(Expression @delegate, string name, string code)
        {
            return Expression.New(_New, @delegate, Expression.Constant(name), Expression.Constant(code));
        }
    }


    public class JSGeneratorFunctionBuilder
    {
        private static Type type = typeof(JSGeneratorFunction);

        private static ConstructorInfo _New =
            type.Constructor(typeof(JSGeneratorDelegate), typeof(string), typeof(string));

        public static Expression New(Expression @delegate, string name, string code)
        {
            return Expression.New(_New, @delegate, Expression.Constant(name), Expression.Constant(code));
        }
    }
}
