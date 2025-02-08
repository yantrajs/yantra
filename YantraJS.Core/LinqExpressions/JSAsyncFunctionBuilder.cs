using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core.Core.Generator;
using YantraJS.Core.LambdaGen;
using YantraJS.Core.LinqExpressions.GeneratorsV2;
using YantraJS.Expressions;

namespace YantraJS.Core.LinqExpressions
{
    public static class JSAsyncFunctionBuilder
    {

        public static YExpression Create(YExpression fx)
        {
            return NewLambdaExpression.StaticCallExpression<JSFunction>(() => () => JSAsyncFunction.Create((JSGeneratorFunctionV2)null), fx);
        }

    }
}
