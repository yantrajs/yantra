using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSRegExpBuilder : TypeHelper<Core.JSRegExp>
    {
        private static ConstructorInfo _New = Constructor<string, string>();

        public static Expression New(Expression exp, Expression exp2)
        {
            return Expression.New(_New, exp, exp2);
        }

    }
}
