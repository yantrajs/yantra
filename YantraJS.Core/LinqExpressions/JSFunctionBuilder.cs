﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using YantraJS.Core.String;
using YantraJS.Extensions;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.LambdaGen;

namespace YantraJS.ExpHelper
{
    //public class JSClosureFunctionBuilder
    //{
    //    private static readonly Type type = typeof(JSClosureFunction);

    //    private static readonly ConstructorInfo _New =
    //        type.Constructor(
    //            typeof(ScriptInfo),
    //            typeof(JSVariable[]),
    //            typeof(JSClosureFunctionDelegate),
    //            StringSpanBuilder.RefType,
    //            StringSpanBuilder.RefType,
    //            typeof(int));

    //    internal static Expression New(
    //        Expression scriptInfo,
    //        Expression closureArray, 
    //        Expression lambda, 
    //        Expression fxName, 
    //        Expression code, 
    //        int count)
    //    {
    //        return Expression.New(_New, scriptInfo, closureArray,
    //            lambda,
    //            fxName,
    //            code,
    //            Expression.Constant(count));
    //    }
    //}

    public class JSFunctionBuilder
    {
        static Type type = typeof(JSFunction);

        public static FieldInfo _prototype =
            type.PublicField(nameof(JSFunction.prototype));

        public static Expression Prototype(Expression target)
        {
            return Expression.Field(target, _prototype);
        }


        private static ConstructorInfo _New =
            type.Constructor(new Type[] { typeof(JSFunctionDelegate), 
                StringSpanBuilder.RefType, 
                StringSpanBuilder.RefType, typeof(int), typeof(bool) });

        private static FieldInfo _f =
            type.InternalField(nameof(JSFunction.f));

        private static MethodInfo invokeFunction =
            typeof(JSValue).InternalMethod(nameof(JSFunction.InvokeFunction), ArgumentsBuilder.refType);

        private static MethodInfo _invokeSuperConstructor
            = typeof(JSFunction).PublicMethod(nameof(JSFunction.InvokeSuperConstructor), 
                typeof(JSValue), 
                typeof(JSValue),
                typeof(Arguments).MakeByRefType());

        public static Expression InvokeSuperConstructor(
            Expression super,
            Expression returnValue, Expression args)
        {
            return Expression.Assign(returnValue,
                super.CallExpression<JSFunction, JSValue>((x) => x.InvokeSuper(Arguments.Empty), args)
                );
            //return Expression.Assign(returnValue, 
            //    Expression.Call(null, _invokeSuperConstructor, newTarget, super, args));
        }

        public static Expression InvokeFunction(Expression target, Expression args, bool coalesce = false)
        {
            // var asFunction = Expression.Coalesce(Expression.TypeAs(target, typeof(JSFunction)),
            //    JSExceptionBuilder.ThrowNotFunction(target));
            // var field = Expression.Field(asFunction, _f);
            // return Expression.Invoke(field, t, args);
            if (coalesce)
            {
                var pes = Expression.Parameters(typeof(JSValue));
                var pe = pes[0];
                return Expression.Block(
                    pes.AsSequence(),
                    Expression.Assign(pe, target),
                    Expression.Condition(JSValueBuilder.IsNullOrUndefined(pe),
                    JSUndefinedBuilder.Value,
                    Expression.Call(pe, invokeFunction, args)));
            }
            return Expression.Call(target, invokeFunction, args);
        }

        public static Expression New(Expression del, Expression name, Expression code, int length)
        {
            return Expression.New(_New , del, 
                name, 
                code,
                Expression.Constant(length),
                Expression.Constant(true));
        }
    }

}
