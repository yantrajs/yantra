using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core.Clr;
using YantraJS.ExpHelper;
using YantraJS.LinqExpressions;
using YantraJS.Runtime;
using Expression = YantraJS.Expressions.YExpression;

namespace YantraJS.Core;

internal static class ClrTypeBuilder
{

    internal delegate JSValue InstanceDelegate<T>(T @this, in Arguments a);

    internal delegate object ClrProxyFactory(in Arguments a);

    private static JSFunctionDelegate CreateInstanceDelegate<T>(this MethodInfo method)
    {
        var d = method.CreateDelegate<InstanceDelegate<T>>();
        var thisDelegate = JSValueToClrConverter.ToFastClrDelegate<T>();
        return (in Arguments a) => {
            var @this = thisDelegate(a.This, "this");
            return d(@this, in a);
        };
    }
    internal static ClrProxyFactory CompileToJSFunctionDelegate(this ConstructorInfo m, string name = null)
    {
        var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
        var parameters = m.GetArgumentsExpression(args);
        Expression body = Expression.New(m, parameters);
        body = m.DeclaringType.IsValueType
            ? Expression.Box(body)
            : body;
        var lambda = Expression.Lambda<ClrProxyFactory>(name, body, args);
        return lambda.Compile();
    }

    internal static JSFunctionDelegate CompileToJSFunctionDelegate(this MethodInfo m, string name = null)
    {

        if (m.IsJSFunctionDelegate()) {
            if (m.IsStatic)
            {
                return (JSFunctionDelegate)m.CreateDelegate(typeof(JSFunctionDelegate));
            }
            else
            {
                // we can directly create a delegate here...
                return Generic.InvokeAs(m.DeclaringType, CreateInstanceDelegate<object>, m);
            }
        }

        // We cannot use delegates as Arguments to CLR and CLR to JSValue
        // will be slower as it will use reflection internally to dispatch
        // actual conversion method.

        name ??= m.Name.ToCamelCase();

        // To speed up, we will use compilation.

        var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
        var parameters = m.GetArgumentsExpression(args);

        Expression body;

        Type returnType;

        var @this = ArgumentsBuilder.This(args);
        var convertedThis = m.IsStatic
            ? null
            : JSValueToClrConverter.Get(@this, m.DeclaringType, "this");
        body = Expression.Call(convertedThis, m, parameters);
        returnType = m.ReturnType;

        // unless return type is JSValue
        // we need to marshal it
        if (returnType == typeof(void))
        {
            body = Expression.Block(body, JSUndefinedBuilder.Value);
        }
        else
        {
            body = ClrProxyBuilder.Marshal(body);
        }

        var lambda = Expression.Lambda<JSFunctionDelegate>(name, body, args);
        return lambda.Compile();
    }

    private static List<Expression> GetArgumentsExpression(this MethodBase m, Expression args)
    {
        var parameters = new List<Expression>();
        var pList = m.GetParameters();
        for (int i = 0; i < pList.Length; i++)
        {
            var ai = ArgumentsBuilder.GetAt(args, i);
            var pi = pList[i];
            Expression defValue;
            if (pi.HasDefaultValue)
            {
                defValue = Expression.Constant(pi.DefaultValue);
                if (pi.ParameterType.IsValueType)
                {
                    defValue = Expression.Box(Expression.Constant(pi.DefaultValue));
                }
                parameters.Add(JSValueToClrConverter.GetArgument(args, i, pi.ParameterType, defValue, pi.Name));
                continue;
            }
            defValue = null;
            if (pi.ParameterType.IsValueType)
            {
                defValue = Expression.Constant(Activator.CreateInstance(pi.ParameterType));
            }
            else
            {
                defValue = Expression.Null;
            }
            parameters.Add(JSValueToClrConverter.GetArgument(args, i, pi.ParameterType, defValue, pi.Name));
        }
        return parameters;
    }

}
