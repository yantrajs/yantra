using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Generator;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSGeneratorBuilder
    {
        private static Type type = typeof(JSGenerator);
        private static MethodInfo yield = type.GetMethod(nameof(JSGenerator.Yield), new Type[] { typeof(JSValue) });
        private static MethodInfo @delegate = type.GetMethod(nameof(JSGenerator.Delegate), new Type[] { typeof(JSValue) });

        public static Expression Yield(Expression generator, Expression value)
        {
            return Expression.Call(generator, yield, value);
        }

        public static Expression Delegate(Expression generator, Expression value)
        {
            return Expression.Call(generator, @delegate, value);
        }

    }
}
