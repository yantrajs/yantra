using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public static class JSArgumentsBuilder
    {
        private static Type type = typeof(JSArguments);
        private static ConstructorInfo _New
            = type.Constructor(new Type[] { typeof(Arguments).MakeByRefType() });

        public static Expression New(Expression args)
        {
            return Expression.New(_New, args);
        }
    }
}
