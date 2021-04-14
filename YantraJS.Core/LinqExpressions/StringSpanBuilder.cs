using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public static class StringSpanBuilder
    {

        public static Type RefType = typeof(StringSpan).MakeByRefType();

        public static Type type = typeof(StringSpan);

        private static ConstructorInfo _new =
            type.Constructor(typeof(string), typeof(int), typeof(int));

        internal static Expression New(Expression code, int start, int v)
        {
            return Expression.New(_new, code, Expression.Constant(start), Expression.Constant(v));
        }

        internal static Expression New(in StringSpan code)
        {
            return Expression.New(_new, 
                Expression.Constant(code.Source), 
                Expression.Constant(code.Offset), 
                Expression.Constant(code.Length));
        }


        public static readonly Expression Empty = 
            Expression.Field(null, type.GetField(nameof(StringSpan.Empty)));
    }
}
