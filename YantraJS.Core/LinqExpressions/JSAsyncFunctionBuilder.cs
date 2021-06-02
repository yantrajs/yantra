using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core.Core.Generator;
using YantraJS.Core.LinqExpressions.GeneratorsV2;
using YantraJS.Expressions;

namespace YantraJS.Core.LinqExpressions
{
    public static class JSAsyncFunctionBuilder
    {

        static Type type = typeof(JSAsyncFunction);

        static MethodInfo method = type.PublicMethod(nameof(JSAsyncFunction.Create), typeof(JSGeneratorFunctionV2));

        public static YExpression Create(YExpression fx)
        {
            return YExpression.Call(null, method, fx);
        }

    }
}
