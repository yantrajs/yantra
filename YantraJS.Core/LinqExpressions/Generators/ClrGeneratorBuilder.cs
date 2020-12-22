using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public static class ClrGeneratorBuilder
    {
        internal static Expression ToLambda(this Expression body)
        {
            if (body == null)
                return Expression.Constant(null);
            return Expression.Lambda(body);
        }

        internal static bool HasYield(this Expression exp)
        {
            if (exp == null)
                return false;
            return YieldFinder.ContainsYield(exp);
        }

        internal static Expression CastAs(this Expression target, Type type)
        {
            if (target == null)
                return null;
            if (type == typeof(object))
                return target;
            if (target.Type == typeof(void)) {
                return Expression.Block(target, Expression.Default(type));
            }
            if (type == target.Type)
                return target;
            if (type.IsValueType)
                return Expression.Convert(target, type);
            return Expression.TypeAs(target, type);
        }

        private static Type type = typeof(ClrGenerator);

        internal static MethodInfo _block = type.GetMethod(nameof(ClrGenerator.Block));
        internal static MethodInfo _binary = type.GetMethod(nameof(ClrGenerator.Binary));
        internal static MethodInfo _if = type.GetMethod(nameof(ClrGenerator.If));

        public static Expression Block(Expression generator, IEnumerable<Expression> lambda)
        {
            return Expression.Call(generator, _block, lambda);
        }

        public static Expression Binary(
            Expression generator, 
            Expression left,
            Type leftType,
            Expression right, 
            Type rightType,
            BinaryExpression final)
        {
            var m = _binary.MakeGenericMethod(left.Type, right.Type);
            var pLeft = Expression.Parameter(leftType);
            var pRight = Expression.Parameter(rightType);
            var body = Expression.Lambda(final.Update(pLeft, final.Conversion, pRight), pLeft, pRight);
            return Expression.Call(generator, m, left, right, body);
        }
    }
}
