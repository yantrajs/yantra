using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using System.Reflection;
using System.Diagnostics.Contracts;

namespace YantraJS.Core.LambdaGen;

internal static class NewLambdaExpression
{

    public static Expression FieldExpression<TTarget, TTOut>(
        this Expression exp,
        Expression<Func<TTarget, TTOut>> func)
        where TTarget : class
    {
        if (!((func.Body as MemberExpression).Member is FieldInfo f))
            throw new InvalidOperationException("Express isn't a field");
        if (f.IsStatic)
        {
            return Expression.Field(null, f);
        }
        return Expression.Field(exp, f);
    }


    public static Expression PropertyExpression<TTarget, TTOut>(
        this Expression exp,
        Expression<Func<TTarget, TTOut>> func)
            where TTarget : class
    {
        if (!((func.Body as MemberExpression).Member is PropertyInfo property))
            throw new InvalidOperationException("Express isn't a property");
        if ((property.GetMethod?.IsStatic ?? false) || (property.SetMethod?.IsStatic ?? false))
        {
            return Expression.Property(null, property);
        }
        return Expression.Property(exp, property);
    }

    public static Expression NewExpression<TOut>(
        Expression<Func<TOut>> fx)
    {
        var m = (fx.Body as NewExpression).Constructor;
        return Expression.New(m);
    }

    public static Expression NewExpression<T1, TOut>(
        Expression<Func<T1, TOut>> fx,
        Expression p1)
    {
        var m = (fx.Body as NewExpression).Constructor;
        return Expression.New(m, p1);
    }

    public static Expression NewExpression<T1, T2, TOut>(
        Expression<Func<T1, T2, TOut>> fx,
        Expression p1,
        Expression p2)
    {
        var m = (fx.Body as NewExpression).Constructor;
        return Expression.New(m, p1, p2);
    }

    public static Expression NewExpression<T1, T2, T3, TOut>(
        Expression<Func<T1, T2, T3, TOut>> fx,
        Expression p1,
        Expression p2,
        Expression p3)
    {
        var m = (fx.Body as NewExpression).Constructor;
        return Expression.New(m, p1, p2, p3);
    }

    public static Expression NewExpression<T1, T2, T3, T4, TOut>(
    Expression<Func<T1, T2, T3, T4, TOut>> fx,
    Expression p1,
    Expression p2,
    Expression p3,
    Expression p4)
    {
        var m = (fx.Body as NewExpression).Constructor;
        return Expression.New(m, p1, p2, p3, p4);
    }

    public static Expression NewExpression<T1, T2, T3, T4, T5, TOut>(
        Expression<Func<T1, T2, T3, T4, T5, TOut>> fx,
        Expression p1,
        Expression p2,
        Expression p3,
        Expression p4,
        Expression p5)
    {
        var m = (fx.Body as NewExpression).Constructor;
        return Expression.New(m, p1, p2, p3, p4, p5);
    }

    public static Expression NewExpression<TOut>(
        Expression<Func<TOut>> fx,
        params Expression[] args)
    {
        var m = (fx.Body as NewExpression).Constructor;
        return Expression.New(m, args);
    }

    public static Expression StaticCallExpression<TIn, TOut>(
        Expression<Func<TIn, TOut>> fx,
        params Expression[] args)
    {
        var m = (fx.Body as MethodCallExpression).Method;
        return Expression.Call(null, m, args);
    }

    public static Expression CallExpression<TIn, TOut>(
        this Expression @this,
        Expression<Func<TIn, TOut>> fx,
        params Expression[] args)
    {
        var m = (fx.Body as MethodCallExpression).Method;
        return Expression.Call(@this, m, args);
    }
}
