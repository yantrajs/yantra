using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebAtoms.CoreJS.ExpHelper
{
    internal class KeyStringsBuilder
    {
        private static MethodInfo _GetOrAdd =
            typeof(Core.KeyStrings).GetMethod("GetOrCreate");

        public static Expression GetOrCreate(Expression text)
        {
            return Expression.Call(null, _GetOrAdd, text);
        }
    }
}
