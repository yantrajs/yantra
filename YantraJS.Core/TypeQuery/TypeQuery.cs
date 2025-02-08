using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YantraJS.Core.Types;

public static class TypeQuery
{

    // we will try to reduce cache
    private static Dictionary<MethodInfo, object> cache = new Dictionary<MethodInfo, object>();

    private static T GetOrCreate<T>(MethodInfo method, Func<T> fx)
    {
        if (cache.TryGetValue(method, out var r))
        {
            return (T)r;
        }
        var rt = fx();
        cache[method] = rt;
        return rt;
    }

    public static ConstructorInfo QueryConstructor<T>(Func<Expression<Func<T>>> fx)
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not NewExpression ne)
                throw new ArgumentException($"Constructor not found in {exp.Type} {exp}");
            return ne.Constructor;
        });

    }

    public static FieldInfo QueryInstanceField<T, TReturn>(Func<Expression<Func<T, TReturn>>> fx)
        where T : class
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MemberExpression me)
                throw new ArgumentException($"Field not found in {exp}");
            if (me.Member is not FieldInfo field)
                throw new ArgumentException($"{me.Member} is not a field");
            return field;
        });

    }

    public static FieldInfo QueryStaticField<TReturn>(Func<Expression<Func<TReturn>>> fx)
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MemberExpression me)
                throw new ArgumentException($"Field not found in {exp}");
            if (me.Member is not FieldInfo field)
                throw new ArgumentException($"{me.Member} is not a field");
            return field;
        });

    }

    public static PropertyInfo QueryInstanceProperty<T, TReturn>(Func<Expression<Func<T, TReturn>>> fx)
        where T : class
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MemberExpression me)
                throw new ArgumentException($"Field not found in {exp}");
            if (me.Member is not PropertyInfo field)
                throw new ArgumentException($"{me.Member} is not a field");
            return field;
        });

    }

    public static MethodInfo QueryStaticMethod<T>(Func<Expression<Func<T>>> fx)
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MethodCallExpression me)
                throw new ArgumentException($"Method found in {exp}");
            return me.Method;
        });

    }

    public static MethodInfo QueryStaticMethod(Func<Expression<Action>> fx)
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MethodCallExpression me)
                throw new ArgumentException($"Method found in {exp}");
            return me.Method;
        });
    }

    public static MethodInfo QueryInstanceMethod<T, TOut>(Func<Expression<Func<T, TOut>>> fx)
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MethodCallExpression me)
                throw new ArgumentException($"Method found in {exp}");
            return me.Method;
        });

    }

    public static MethodInfo QueryInstanceMethod<T>(Func<Expression<Action<T>>> fx)
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MethodCallExpression me)
                throw new ArgumentException($"Method found in {exp}");
            return me.Method;
        });

    }

    public static MethodInfo QueryInstanceMethod(Func<LambdaExpression> fx)
    {
        return GetOrCreate(fx.Method, () => {
            var exp = fx();
            if (exp.Body is not MethodCallExpression me)
                throw new ArgumentException($"Method found in {exp}");
            return me.Method;
        });

    }

    private static ConstructorInfo QueryConstructor<T>(Expression<Func<T>> expression)
    {
        if (!(expression is NewExpression ne))
            throw new ArgumentException($"Constructor not found in {expression}");
        return ne.Constructor;
    }

    private static MethodInfo QueryStaticMethod<T>(Expression<Func<T>> expression)
    {
        if (!(expression is MethodCallExpression method))
            throw new ArgumentException($"Method not found in {expression}");
        return method.Method;
    }

    private static MethodInfo QueryInstanceMethod<T, TReturn>(Expression<Func<T, TReturn>> expression)
    {
        if (!(expression is MethodCallExpression method))
            throw new ArgumentException($"Method not found in {expression}");
        return method.Method;
    }

}

