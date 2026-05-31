using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using System.Reflection;
using System.Diagnostics.Contracts;
using YantraJS.Core.Types;
using YantraJS.Expressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;

namespace YantraJS.Core.LambdaGen;

internal static class NewLambdaExpression
{

    public static Expression FieldExpression<TTarget, TTOut>(
        this Expression exp,
        Func<Expression<Func<TTarget, TTOut>>> func)
        where TTarget : class
    {
        var f = TypeQuery.QueryInstanceField(func);
        if (f.IsStatic)
        {
            return Expression.Field(null, f);
        }
        return Expression.Field(exp, f);
    }


    public static Expression StaticFieldExpression<TTOut>(
        Func<Expression<Func<TTOut>>> func)
    {
        var f = TypeQuery.QueryStaticField(func);
        return Expression.Field(null, f);
    }

    public static Expression PropertyExpression<TTarget, TTOut>(
        this Expression exp,
        Func<Expression<Func<TTarget, TTOut>>> func)
    where TTarget : class
    {
        var f = TypeQuery.QueryInstanceProperty(func);
        return Expression.Property(exp, f);
    }

    public static YNewExpression NewExpression<TOut>(
        Func<Expression<Func<TOut>>> fx,
        params Expression[] args)
    {
        var m = TypeQuery.QueryConstructor(fx);
        return Expression.New(m, args);
    }

    public static Expression StaticCallExpression<TOut>(
        Func<Expression<Func<TOut>>> fx,
        params Expression[] args)
    {
        var m = TypeQuery.QueryStaticMethod(fx);
        return Expression.Call(null, m, args);
    }
    public static Expression StaticCallExpression(
    Func<Expression<Action>> fx,
    params Expression[] args)
    {
        var m = TypeQuery.QueryStaticMethod(fx);
        return Expression.Call(null, m, args);
    }

    public static Expression CallExpression<TIn, TOut>(
        this Expression @this,
        Func<Expression<Func<TIn, TOut>>> fx,
        params Expression[] args)
    {
        var m = TypeQuery.QueryInstanceMethod(fx);
        if (m.IsStatic)
        {
            if (args.Length == 0)
            {
                return Expression.Call(null, m, @this);
            }
            var newArgs = new Expression[args.Length + 1];
            newArgs[0] = @this;
            Array.Copy(args, 0, newArgs, 1, args.Length);
            return Expression.Call(null, m, newArgs);
        }
        return Expression.Call(@this, m, args);
    }

    public static Expression CallExpression<T>(
        this Expression @this,
        Func<Expression<Action<T>>> fx,
        params Expression[] args)
    {
        var m = TypeQuery.QueryInstanceMethod(fx);
        return Expression.Call(@this, m, args);
    }

    public static Expression CallExpression<T1, T2>(
        this Expression @this,
        Func<Expression<Action<T1, T2>>> fx,
        params Expression[] args)
    {
        var m = TypeQuery.QueryInstanceMethod(fx);
        return Expression.Call(@this, m, args);
    }

    public static Expression CallExpression<TIn, T, TOut>(
        this Expression @this,
        Func<Expression<Func<TIn, T, TOut>>> fx,
        Expression p1)
    {
        var m = TypeQuery.QueryInstanceMethod(fx);
        return Expression.Call(@this, m, p1);
    }


    public static Expression CallExpression<TIn, T1, T2>(
        this Expression @this,
        Func<Expression<Action<TIn, T1, T2>>> fx,
        Expression p1, Expression p2)
    {
        var m = TypeQuery.QueryInstanceMethod(fx);
        return Expression.Call(@this, m, p1, p2);
    }

    public static Expression MakeIndexExpression<TIn, TIndex1, TIndex2, TOut>(
        this Expression @this,
        Func<Expression<Func<TIn, TIndex1, TIndex2, TOut>>> fx,
        Expression p1,
        Expression p2
    )
    {
        var m = TypeQuery.QueryInstanceIndex(fx);
        return Expression.MakeIndex(@this, m, p1, p2);
    }

    public static Expression MakeIndexExpression<TIn, TIndex1, TOut>(
        this Expression @this,
        Func<Expression<Func<TIn, TIndex1, TOut>>> fx,
        Expression p1
    )
    {
        var m = TypeQuery.QueryInstanceIndex(fx);
        return Expression.MakeIndex(@this, m, p1);
    }

    public static Expression As<T>(this Expression @this)
    {
        return Expression.Convert(@this, typeof(T));
    }

    public static bool TryReduceToDouble(this Expression exp, out Expression output)
    {
        if (exp.Type == typeof(double))
        {
            output = exp;
            return true;
        }
        if (exp is YTypeAsExpression typeAs)
        {
            exp = typeAs.Target;
        }
        if (exp is YNewExpression newExp)
        {
            if (newExp.Type == typeof(JSNumber))
            {
                var arg0 = newExp.args[0];
                if (arg0?.Type == typeof(double))
                {
                    output = arg0;
                    return true;
                }
            }
        }
        output = default;
        return false;
    }

    public static bool TryReduceToString(this Expression exp, out Expression output)
    {
        if (exp.Type == typeof(string))
        {
            output = exp;
            return true;
        }
        if (exp is YTypeAsExpression typeAs)
        {
            exp = typeAs.Target;
        }
        if (exp is YNewExpression newExp)
        {
            if (newExp.Type == typeof(JSString))
            {
                var arg0 = newExp.args[0];
                if (arg0?.Type == typeof(string))
                {
                    output = arg0;
                    return true;
                }
            }
        }
        output = default;
        return false;
    }

    public static bool TryReduceToBoolean(this Expression exp, out Expression output)
    {
        if (exp.Type == typeof(bool))
        {
            output = exp;
            return true;
        }
        if (exp is YTypeAsExpression typeAs)
        {
            exp = typeAs.Target;
        }
        if (exp is YConditionalExpression ce)
        {
            if (ce.@true == JSBooleanBuilder.True && ce.@false == JSBooleanBuilder.False) {
                output = ce.test;
                return true;
            }
            if (ce.@true == JSBooleanBuilder.False && ce.@false == JSBooleanBuilder.True) {
                output = Expression.Not( ce.test);
                return true;
            }
        }
        if (exp == JSBooleanBuilder.True)
        {
            output = YExpression.Constant(true);
            return true;
        }
        if (exp == JSBooleanBuilder.False)
        {
            output = YExpression.Constant(false);
            return true;
        }
        output = default;
        return false;
    }

    public static bool TryReduceToLiteral(this Expression exp, out Expression doubleValue, out Expression stringValue)
    {
        if (exp.TryReduceToDouble(out doubleValue))
        {
            stringValue = default;
            return true;
        }
        if (exp.TryReduceToString(out stringValue)) {
            doubleValue = default;
            return true;
        }
        doubleValue = default;
        stringValue = default;
        return false;
    }

    public static Expression ToIntValue(this Expression exp)
    {
        var toLong = Expression.Convert(exp, typeof(long), true);
        var toLeft = Expression.LeftShift(toLong, Expression.Constant(32));
        var toRight = Expression.RightShift(toLeft, Expression.Constant(32));
        return Expression.Convert(toRight, typeof(int), true);
    }

}
