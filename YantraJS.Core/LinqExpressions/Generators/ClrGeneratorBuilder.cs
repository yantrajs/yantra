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
            if (body.NodeType == ExpressionType.Default && body.Type == typeof(void))
                return Expression.Constant(null, typeof(Func<object>));
            if (body.Type == typeof(Func<object>))
                return body;
            return Expression.Lambda(typeof(Func<object>), 
                body.Type.IsValueType 
                ? Expression.Convert(body,typeof(object)) 
                : body);
        }

        internal static Expression ToLambda(this Expression body, Type type)
        {
            if (body == null)
                return Expression.Constant(null);
            var t = typeof(Func<>).MakeGenericType(type);
            if (body.NodeType == ExpressionType.Lambda)
            {
                var l = body as LambdaExpression;
                if (l.ReturnType == typeof(object))
                {
                    var lb = l.Body;
                    return Expression.Lambda(t, Expression.Convert(lb, type));
                }
            }
            if (body.Type.IsFunc())
            {
                var p = Expression.Parameter(body.Type, "func");
                return Expression.Lambda(t, Expression.Block(
                    new ParameterExpression[] {p},
                    Expression.Assign(p, body),
                    Expression.Convert( Expression.Invoke(body),type)
                    ));
            }

            return Expression.Lambda(t, Expression.Convert(body,type));
        }


        internal static bool HasYield(this Expression exp)
        {
            if (exp == null)
                return false;
            return exp.GetExtendedValue().HasYield;
        }

        internal static bool ShouldBreak(this Expression exp)
        {
            if (exp == null)
                return false;
            var e = exp.GetExtendedValue();
            return e.HasYield || e.ForceBreak;
        }


        internal static Expression AsObject(this Expression target)
        {
            if (target.Type == typeof(object))
                return target;
            if (target.NodeType == ExpressionType.Default && target.Type == typeof(void))
                return Expression.Constant(null, typeof(object));
            return Expression.TypeAs(target, typeof(object));
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
            return Expression.Call(generator, _block, Expression.NewArrayInit(typeof(Func<object>), lambda));
        }

        public static Expression Binary(
            Expression generator, 
            ParameterExpression leftParameter,
            Expression left,
            Type leftType,
            Expression right, 
            Type rightType,
            BinaryExpression final)
        {
            var m = _binary.MakeGenericMethod(leftParameter.Type, rightType);
            // var pLeft = Expression.Parameter(leftType);
            var pRight = Expression.Parameter(rightType);
            var body = Expression.Lambda(final.Update(left, final.Conversion, pRight), leftParameter, pRight);
            return Expression.Call(generator, m, left, right, body);
        }
    }
}
