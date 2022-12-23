using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Core.LinqExpressions
{
    internal class JSDateBuilder
    {

        public readonly static Type type = typeof(JSDate);

        private static PropertyInfo _dateTime =  type.GetProperty(nameof(JSDate.DateTime));

        private static PropertyInfo _value = type.GetProperty(nameof(JSDate.Value));

        public static YExpression Value(YExpression target) => YExpression.Property(target, _value);

        public static YExpression DateTime(YExpression target) => YExpression.Property(target, _dateTime);
    }
}
