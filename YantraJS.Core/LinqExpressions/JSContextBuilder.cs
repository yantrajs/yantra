using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSContextBuilder
    {

        private static Type type = typeof(JSContext);


        public static Expression Current =
            Expression.Property(null, type.Property(nameof(JSContext.Current)));

        public static Expression Object =
            Expression.Field(Current, type.GetField(nameof(JSContext.Object)));

        public static MethodInfo _Push =
            type.InternalMethod(nameof(JSContext.Push), typeof(string), typeof(string), typeof(int), typeof(int));

        public static MethodInfo _Pop=
            type.InternalMethod(nameof(JSContext.Pop));

        public static MethodInfo _Update=
            type.InternalMethod(nameof(JSContext.Update), typeof(int), typeof(int));

        public static MethodInfo _Register =
            type.InternalMethod(nameof(JSContext.Register), typeof(JSVariable));

        private static PropertyInfo _Index =
            type.IndexProperty(typeof(Core.KeyString));
        public static Expression Index(Expression key)
        {
            return Expression.MakeIndex(Current, _Index, new Expression[] { key });
        }

        public static Expression Pop(Expression context)
        {
            return Expression.Call(context, _Pop);
        }

        public static Expression Push(
            Expression fileName, 
            string str, 
            int line, 
            int column)
        {
            return Expression.Call(
                Current,
                _Push,
                fileName,
                Expression.Constant(str),
                Expression.Constant(line),
                Expression.Constant(column));
        }
        public static Expression Update(ParameterExpression s, int line, int column)
        {
            return Expression.Call(
                s,
                _Update,
                Expression.Constant(line),
                Expression.Constant(column));
        }
        public static Expression Register(ParameterExpression lScope, ParameterExpression variable)
        {
            return Expression.Call(lScope, _Register, variable);
        }
    }
}
