using System;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.BigInt;
using YantraJS.Core.LambdaGen;
using YantraJS.Expressions;

namespace YantraJS.ExpHelper
{
    public class JSBigIntBuilder
    {
        internal static YExpression New(string value)
        {
            return NewLambdaExpression.NewExpression<JSBigInt>(() => () => new JSBigInt("a"),
                YExpression.Constant(value)
                );
        }
    }

    public class JSDecimalBuilder
    {
        internal static YExpression New(string value)
        {
            return NewLambdaExpression.NewExpression<JSDecimal>(() => () => new JSDecimal("a"),
                YExpression.Constant(value)
                );
        }
    }
}
