using Esprima;
using System;
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

namespace YantraJS.ExpHelper
{
    public class JSClosureFunctionBuilder
    {
        private static readonly Type type = typeof(JSClosureFunction);

        private static readonly ConstructorInfo _New =
            type.Constructor(
                typeof(ScriptInfo),
                typeof(JSVariable[]),
                typeof(JSClosureFunctionDelegate),
                StringSpanBuilder.RefType,
                StringSpanBuilder.RefType,
                typeof(int));

        internal static Expression New(
            Expression scriptInfo,
            Expression closureArray, 
            LambdaExpression lambda, 
            Expression fxName, 
            Expression code, 
            int count)
        {
            return Expression.New(_New, scriptInfo, closureArray,
                lambda,
                fxName,
                code,
                Expression.Constant(count));
        }
    }

    public class JSFunctionBuilder
    {
        static Type type = typeof(JSFunction);

        private static FieldInfo _prototype =
            type.InternalField(nameof(JSFunction.prototype));

        public static Expression Prototype(Expression target)
        {
            return Expression.Field(target, _prototype);
        }


        private static ConstructorInfo _New =
            type.Constructor(new Type[] { typeof(JSFunctionDelegate), 
                StringSpanBuilder.RefType, 
                StringSpanBuilder.RefType, typeof(int) });

        private static FieldInfo _f =
            type.InternalField(nameof(JSFunction.f));

        private static MethodInfo invokeFunction =
            typeof(JSValue).GetMethod(nameof(JSFunction.InvokeFunction));

        private static MethodInfo _invokeSuperConstructor
            = typeof(JSFunction).InternalMethod(nameof(JSFunction.InvokeSuperConstructor), typeof(JSValue), typeof(Arguments).MakeByRefType());

        public static Expression InvokeSuperConstructor(Expression returnValue, Expression super, Expression args)
        {
            return Expression.Assign(returnValue, Expression.Call(null, _invokeSuperConstructor, super, args));
        }

        public static Expression InvokeFunction(Expression target, Expression args)
        {
            // var asFunction = Expression.Coalesce(Expression.TypeAs(target, typeof(JSFunction)),
            //    JSExceptionBuilder.ThrowNotFunction(target));
            // var field = Expression.Field(asFunction, _f);
            // return Expression.Invoke(field, t, args);
            return Expression.Call(target, invokeFunction, args);
        }

        public static Expression New(Expression del, Expression name, Expression code, int length)
        {
            return Expression.New(_New , del, 
                name, 
                code,
                Expression.Constant(length));
        }
    }

}
