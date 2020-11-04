using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.Generator;

namespace YantraJS.ExpHelper
{

    public class JSAwaiterBuilder
    {
        private static Type type = typeof(JSWeakAwaiter);
        private static MethodInfo await = 
            type.GetMethod(nameof(JSWeakAwaiter.Await), new Type[] { typeof(JSValue) });

        public static Expression Await(Expression generator, Expression value)
        {
            return Expression.Call(generator, await, value);
        }


    }

    public class JSGeneratorBuilder
    {
        private static Type type = typeof(JSWeakGenerator);
        private static MethodInfo yield = type.GetMethod(nameof(JSWeakGenerator.Yield), new Type[] { typeof(JSValue) });
        private static MethodInfo @delegate = type.GetMethod(nameof(JSWeakGenerator.Delegate), new Type[] { typeof(JSValue) });

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
