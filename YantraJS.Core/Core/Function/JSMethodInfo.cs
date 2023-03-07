using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using YantraJS.ExpHelper;
using YantraJS.Expressions;
using YantraJS.Generator;
using YantraJS.LinqExpressions;
using YantraJS.Runtime;

namespace YantraJS.Core.Clr
{
    internal class JSMethodInfo
    {
        public readonly MethodInfo Method;

        public readonly string Name;
        public readonly bool Export;

        public JSMethodInfo(ClrMemberNamingConvention namingConvention, MethodInfo method)
        {
            this.Method = method;
            var (name, export) = ClrTypeExtensions.GetJSName(namingConvention, method);
            this.Name = name;
            this.Export = export;
        }

        internal JSValue GenerateInvokeJSFunction()
        {
            return this.InvokeAs(Method.DeclaringType, ToInstanceJSFunctionDelegate<object>);
        }

        public delegate JSValue InstanceDelegate<T>(T @this, in Arguments a);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSFunction ToInstanceJSFunctionDelegate<T>()
        {
            if (Method.IsStatic)
            {
                var staticDel = (JSFunctionDelegate)Method.CreateDelegate(typeof(JSFunctionDelegate));
                return new JSFunction((in Arguments a) =>
                {
                    return staticDel(a);
                }, Name);
            }
            var del = (InstanceDelegate<T>)Method.CreateDelegate(typeof(InstanceDelegate<T>));
            var type = typeof(T);
            return new JSFunction((in Arguments a) =>
            {
                var @this = (T)a.This.ForceConvert(type);
                return del(@this, a);
            }, Name);
        }

        public JSFunctionDelegate GenerateMethod()
        {
            var m = this.Method;
            var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var @this = ArgumentsBuilder.This(args);

            var convertedThis = m.IsStatic
                ? null
                : JSValueToClrConverter.Get(@this, m.DeclaringType, "this");
            var parameters = new List<Expression>();
            var pList = m.GetParameters();
            for (int i = 0; i < pList.Length; i++)
            {
                var pi = pList[i];
                var defValue = pi.HasDefaultValue
                    ? Expression.Constant((object)pi.DefaultValue, typeof(object))
                    : (pi.ParameterType.IsValueType
                        ? Expression.Constant((object)Activator.CreateInstance(pi.ParameterType), typeof(object))
                        : null);
                parameters.Add(JSValueToClrConverter.GetArgument(args, i, pi.ParameterType, defValue, pi.Name));
            }
            var call = Expression.Call(convertedThis, m, parameters);
            var marshal = call.Type == typeof(void)
                ? YExpression.Block(call, JSUndefinedBuilder.Value)
                : ClrProxyBuilder.Marshal(call);
            var wrapTryCatch = JSExceptionBuilder.Wrap(marshal);

            ILCodeGenerator.GenerateLogs = true;
            var lambda = Expression.Lambda<JSFunctionDelegate>(m.Name, wrapTryCatch, args);
            var method = lambda.Compile();
            return method;
        }

    }
}
