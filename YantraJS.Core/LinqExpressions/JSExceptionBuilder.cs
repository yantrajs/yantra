using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using YantraJS.Core;
using YantraJS.Core.LambdaGen;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.ExpHelper
{
    public static class JSExceptionBuilder
    {
        // private static Type type = typeof(JSException);

        //private static ConstructorInfo _new =
        //    type.Constructor(typeof(string), typeof(string), typeof(string), typeof(int));

        //private static MethodInfo _Throw =
        //    type.InternalMethod(nameof(Core.JSException.Throw), typeof(Core.JSValue));

        //private static MethodInfo _ThrowSyntaxError =
        //    type.PublicMethod(nameof(Core.JSException.ThrowSyntaxError), typeof(string));

        //private static MethodInfo _From =
        //    type.InternalMethod(nameof(JSException.From), typeof(Exception));

        public static Expression Throw(Expression value)
        {
            return NewLambdaExpression.StaticCallExpression(
                () => () => JSException.Throw((JSValue)null),
            value);
            //return Expression.Call(null, _Throw, value);
        }

        public static Expression ThrowSyntaxError(string value)
        {
            return NewLambdaExpression.StaticCallExpression(
                () => () => JSException.ThrowSyntaxError(""),
                Expression.Constant(value));
            //return Expression.Call(null, _ThrowSyntaxError, Expression.Constant(value));
        }

        //private static MethodInfo _ThrowNotFunction =
        //    type.InternalMethod(nameof(Core.JSException.ThrowNotFunction), typeof(Core.JSValue));

        //public static Expression ThrowNotFunction(Expression value)
        //{
        //    return Expression.Call(null, _ThrowNotFunction, value);
        //}
        //private static PropertyInfo _Error =
        //    type.Property(nameof(JSException.Error));

        //public static Expression Error(Expression target)
        //{
        //    return Expression.Property(target, _Error);
        //}

        //public static Expression From(Expression ex)
        //{
        //    return Expression.Call(null, _From, ex);
        //}

        public static Expression Throw(string message,
            Type type = null,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
            {
                return Expression.Throw(
                    NewLambdaExpression.NewExpression<JSException>(() => () => new JSException(
                        "",
                        "",
                        "",
                        0
                        ),
                Expression.Constant(message),
                Expression.Constant(function),
                Expression.Constant(filePath),
                Expression.Constant(line)), type ?? typeof(JSValue));
            //return Expression.Throw(Expression.New(_new,
            //        Expression.Constant(message),
            //        Expression.Constant(function),
            //        Expression.Constant(filePath),
            //        Expression.Constant(line)), type ?? typeof(JSValue));
            }


        public static Expression New(string message, 
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return Expression.Throw(
                    NewLambdaExpression.NewExpression<JSException>(() => () => new JSException(
                        "",
                        "",
                        "",
                        0
                        ),
                Expression.Constant(message),
                Expression.Constant(function),
                Expression.Constant(filePath),
                Expression.Constant(line)));
        }

        public static Expression Wrap(Expression body)
        {
            return body;
            //var b = Expression.Variable(typeof(Exception));
            //var cb = Expression.Catch(b, 
            //    Expression.Throw(From(b),typeof(JSValue)), 
            //    Expression.Not(Expression.TypeIs(b, typeof(JSException))));
            //return Expression.Block(new ParameterExpression[] { b }, 
            //    Expression.TryCatch(body, cb )).ToJSValue();
        }
    }
}
