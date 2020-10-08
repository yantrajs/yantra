using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSContextBuilder
    {

        private static Type type = typeof(JSContext);


        public static Expression Current =
            Expression.Property(null, type.Property(nameof(JSContext.Current)));

        public static Expression Object =
            Expression.Field(Current, type.GetField(nameof(JSContext.Object)));


        public static Expression CurrentScope =
            Expression.Field(Current, type.InternalField(nameof(JSContext.Scope)));

        private static PropertyInfo _Index =
            type.IndexProperty(typeof(Core.KeyString));
        public static Expression Index(Expression key)
        {
            return Expression.MakeIndex(Current, _Index, new Expression[] { key });
        }
    }
}
