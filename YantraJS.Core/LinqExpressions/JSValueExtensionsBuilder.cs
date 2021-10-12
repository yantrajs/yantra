using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Extensions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.FastParser;

namespace YantraJS.ExpHelper
{
    public class JSValueExtensionsBuilder
    {
        private readonly static Type type = typeof(JSValueExtensions);

        private static readonly MethodInfo _InstanceOf =
            type.GetMethod(nameof(JSValueExtensions.InstanceOf));
        public static Expression InstanceOf(Expression target, Expression value)
        {
            return Expression.Call(null, _InstanceOf, target, value);
        }

        static readonly MethodInfo _IsIn =
            type.GetMethod(nameof(JSValueExtensions.IsIn));
        public static Expression IsIn(Expression target, Expression value)
        {
            return Expression.Call(null, _IsIn, target, value);
        }

        public static Expression Assign(Expression e, Expression value)
        {
            return Expression.Assign(e, value);
        }

        public static Expression AssignCoalesce(Expression e, Expression value)
        {
            return Expression.Assign(e, Expression.Coalesce( NullIfUndefined(e), value));
        }


        private readonly static MethodInfo _NullIfTrue =
            type.PublicMethod(nameof(JSValueExtensions.NullIfTrue), typeof(JSValue));

        public static Expression NullIfTrue(Expression exp)
        {
            return Expression.Call(null, _NullIfTrue, exp);
        }

        private readonly static MethodInfo _NullIfFalse =
            type.PublicMethod(nameof(JSValueExtensions.NullIfFalse), typeof(JSValue));

        private readonly static MethodInfo _NullIfUndefined =
            type.StaticMethod<JSValue>(nameof(JSValueExtensions.NullIfUndefined));

        private readonly static MethodInfo _NullIfNullOrUndefined =
            type.PublicMethod(nameof(JSValueExtensions.NullIfUndefinedOrNull), typeof(JSValue));

        public static Expression Coalesce(Expression left, Expression right)
        {
            return Expression.Coalesce(Expression.Call(null, _NullIfNullOrUndefined, left), right);
        }

        public static Expression NullIfFalse(Expression exp)
        {
            return Expression.Call(null, _NullIfFalse, exp);
        }
        public static Expression NullIfUndefined(Expression exp)
        {
            return Expression.Call(null, _NullIfUndefined, exp);
        }


        static readonly MethodInfo[] _InvokeMethodKeyString
            = new MethodInfo[] {
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethodSpread), typeof(JSValue), KeyStringsBuilder.RefType, typeof(JSValue[])),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), KeyStringsBuilder.RefType),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), KeyStringsBuilder.RefType, typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), KeyStringsBuilder.RefType, typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), KeyStringsBuilder.RefType, typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), KeyStringsBuilder.RefType, typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), KeyStringsBuilder.RefType, typeof(JSValue[]))
            };
        static readonly MethodInfo[] _InvokeMethodKeyUint
            = new MethodInfo[] {
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethodSpread), typeof(JSValue), typeof(uint), typeof(JSValue[])),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint), typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(uint), typeof(JSValue[]))
            };
        static readonly MethodInfo[] _InvokeMethodJSValue
                    = new MethodInfo[] {
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethodSpread), typeof(JSValue), typeof(JSValue), typeof(JSValue[])),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue), typeof(JSValue)),
                type.PublicMethod(nameof(JSValueExtensions.InvokeMethod), typeof(JSValue), typeof(JSValue), typeof(JSValue[]))
                    };

        public static Expression InvokeMethod(Expression target, Expression method, IFastEnumerable<Expression> args, bool hasSpread)
        {

            var methods = method.Type == typeof(KeyString)
                ? _InvokeMethodKeyString
                : method.Type == typeof(uint)
                    ? _InvokeMethodKeyUint
                    : _InvokeMethodJSValue;


            var m = hasSpread ? methods[0] : methods[args.Count <= 4 ? args.Count + 1 : 6];

            if (!hasSpread && args.Count <= 4)
            {
                var finalArgs = new Sequence<Expression>(args.Count + 2) { 
                    target,
                    method
                };
                finalArgs.AddRange(args);
                // finalArgs[0] = target;
                // finalArgs[1] = method;
                // Array.Copy(args, 0, finalArgs, 2, args.Length);
                // args.Copy(finalArgs, 2);

                return Expression.Call(null, m, finalArgs);
            }
            return Expression.Call(null, m, target, method, Expression.NewArray(typeof(JSValue), args));
        }


    }
}
