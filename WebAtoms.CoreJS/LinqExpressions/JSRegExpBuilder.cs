using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSRegExpBuilder
    {
        private static ConstructorInfo _New = typeof(JSRegExp).Constructor(typeof(string), typeof(string));

        public static Expression New(Expression exp, Expression exp2)
        {
            return Expression.New(_New, exp, exp2);
        }

    }
}
