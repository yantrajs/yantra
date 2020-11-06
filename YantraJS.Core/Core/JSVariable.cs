using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace YantraJS.Core
{
    public class JSVariable
    {
        public JSValue Value;

        static readonly FieldInfo _ValueField =
            typeof(JSVariable).GetField("Value");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSVariable(JSValue v, string name)
        {
            this.Value = v;
            var c = JSContext.Current.Scope.Top;
            // need to do this unless in eval
            // c[name] = this;
            if (c.IsRoot || c.Parent.IsRoot)
            {
                JSContext.Current[name] = v;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSVariable(Exception e, string name)
            : this(e is JSException je 
                  ? je.Error
                  : (new JSException(e.ToString())).Error , name)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSVariable New(in Arguments a, int i, string name)
        {
            return new JSVariable(a.GetAt(i), name);
        }

        internal static Expression ValueExpression(Expression exp)
        {
            return Expression.Field(exp, _ValueField);
        }

    }
}
