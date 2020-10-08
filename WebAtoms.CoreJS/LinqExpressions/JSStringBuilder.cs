using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSStringBuilder : TypeHelper<Core.JSString>
    {
        private static FieldInfo _Value =
            InternalField(nameof(Core.JSString.value));

        public static Expression Value(Expression ex)
        {
            return Expression.Field(ex, _Value);
        }

        private static ConstructorInfo _New = Constructor<string>();

        public static Expression New(Expression exp)
        {
            return Expression.New(_New, exp);
        }

        public static Expression ConcatBasicStrings(Expression left, Expression right)
        {
            return Expression.New(_New, StringBuilder.Concat(left, right));
        }

    }
}
