using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    internal class KeyStringsBuilder
    {
        private static MethodInfo _GetOrAdd =
            typeof(KeyStrings).GetMethod(nameof(KeyStrings.GetOrCreate));

        public static Expression GetOrCreate(Expression text)
        {
            return Expression.Call(null, _GetOrAdd, text);
        }
    }
}
