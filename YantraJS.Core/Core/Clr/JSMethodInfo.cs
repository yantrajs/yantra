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
using YantraJS.Core.Clr;

namespace YantraJS.Core.Core.Clr
{
    internal class JSMethodInfo
    {
        public readonly MethodInfo Method;

        public readonly string Name;
        public readonly bool Export;

        public JSMethodInfo(ClrMemberNamingConvention namingConvention, MethodInfo method)
        {
            Method = method;
            var (name, export) = ClrTypeExtensions.GetJSName(namingConvention, method);
            Name = name;
            Export = export;
        }

        internal JSValue GenerateInvokeJSFunction()
        {
            return this.InvokeAs(Method.DeclaringType, ToInstanceJSFunctionDelegate<object>);
        }

        public delegate JSValue InstanceDelegate<T>(T @this, in Arguments a);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSFunction ToInstanceJSFunctionDelegate<T>()
        {
            return new JSFunction(Method.CompileToJSFunctionDelegate(), Name);
            //if (Method.IsStatic)
            //{
            //    var staticDel = (JSFunctionDelegate)Method.CreateDelegate(typeof(JSFunctionDelegate));
            //    return new JSFunction((in Arguments a) =>
            //    {
            //        return staticDel(a);
            //    }, Name);
            //}
            //var del = (InstanceDelegate<T>)Method.CreateDelegate(typeof(InstanceDelegate<T>));
            //var type = typeof(T);
            //return new JSFunction((in Arguments a) =>
            //{
            //    var @this = (T)a.This.ForceConvert(type);
            //    return del(@this, a);
            //}, Name);
        }

        public JSFunctionDelegate GenerateMethod()
        {
            return Method.CompileToJSFunctionDelegate();
        }

    }
}
