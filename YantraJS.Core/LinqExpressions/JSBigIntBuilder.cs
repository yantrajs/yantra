using System;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.BigInt;
using YantraJS.Expressions;

namespace YantraJS.ExpHelper
{
    public class JSBigIntBuilder
    {
        private static ConstructorInfo _New = typeof(JSBigInt).Constructor(typeof(string));

        internal static YExpression New(string value)
        {
            return YExpression.New(_New, YExpression.Constant(value));
        }
    }
}
