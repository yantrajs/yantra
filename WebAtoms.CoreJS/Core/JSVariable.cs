using System;
using System.Linq.Expressions;
using System.Reflection;

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
            var c = JSContext.Current.Scope.Top;
            c[name] = this;
            if (c.IsRoot)
            {
                JSContext.Current[name] = v;
            }
        }

        public JSVariable(Exception e, string name)
            : this(e is JSException je 
                  ? je.Error
                  : new JSString(e.ToString()) , name)
        {

        }

        internal static Expression ValueExpression(Expression exp)
        {
            return Expression.Field(exp, _ValueField);
        }

    }
}
