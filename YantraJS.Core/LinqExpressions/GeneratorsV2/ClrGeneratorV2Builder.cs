using System;
using System.Reflection;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public class ClrGeneratorV2Builder
    {
        private static Type type = typeof(ClrGeneratorV2);

        private static MethodInfo _throw = type.PublicMethod(nameof(ClrGeneratorV2.Throw), typeof(int));
        private static MethodInfo _beginCatch = type.PublicMethod(nameof(ClrGeneratorV2.BeginCatch));
        private static MethodInfo _beginFinally = type.PublicMethod(nameof(ClrGeneratorV2.BeginFinally));

        private static MethodInfo _push = type.PublicMethod(
            nameof(ClrGeneratorV2.PushTry),
            typeof(int),
            typeof(int),
            typeof(int));

        private static MethodInfo _pop = type.PublicMethod(
            nameof(ClrGeneratorV2.Pop));


        private static MethodInfo _GetVariable
            = type.GetMethod("GetVariable");
        private static MethodInfo _InitVariables
            = type.GetMethod("InitVariables");


        public static Expression Push(Expression exp, int c, int f, int e)
        {
            return Expression.Call(exp, _push,
                Expression.Constant(c), 
                Expression.Constant(f),
                Expression.Constant(e));
        }

        internal static Expression GetVariable(ParameterExpression pe, int id, Type type)
        {
            return Expression.Call(pe, _GetVariable.MakeGenericMethod(type), Expression.Constant(id));
        }

        internal static Expression InitVariables(ParameterExpression pe, int count)
        {
            return Expression.Call(pe, _InitVariables, Expression.Constant(count));
        }

        internal static Expression Pop(ParameterExpression pe)
        {
            return Expression.Call(pe, _pop);
        }
        internal static Expression BeginCatch(ParameterExpression pe)
        {
            return Expression.Call(pe, _beginCatch);
        }
        internal static Expression BeginFinally(ParameterExpression pe)
        {
            return Expression.Call(pe, _beginFinally);
        }
        internal static Expression Throw(ParameterExpression pe, int id)
        {
            return Expression.Call(pe, _throw, Expression.Constant(id));
        }
    }
}
