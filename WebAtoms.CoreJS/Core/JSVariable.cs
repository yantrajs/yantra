using System;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.LinqExpressions;

namespace WebAtoms.CoreJS.Core
{
    public class JSVariable
    {
        public JSValue Value;

        private static FieldInfo _ValueField =
            typeof(JSVariable).GetField("Value");

        public JSVariable(JSValue v, string name)
        {
            this.Value = v;
            var c = JSContext.Current.Scope;
            c[name] = this;
            if (c.IsRoot)
            {
                JSContext.Current[name] = v;
            }
        }

        internal static Expression ValueExpression(Expression exp)
        {
            return Expression.Field(exp, _ValueField);
        }

    }
}
