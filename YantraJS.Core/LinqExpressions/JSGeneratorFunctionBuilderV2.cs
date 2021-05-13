using System;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions.GeneratorsV2;

namespace YantraJS.ExpHelper
{
    public class JSGeneratorFunctionBuilderV2
    {
        private static Type type = typeof(JSGeneratorFunctionV2);

        private static ConstructorInfo _New =
            type.Constructor(
                typeof(ScriptInfo),
                typeof(JSVariable[]), typeof(JSGeneratorDelegateV2), StringSpanBuilder.RefType, StringSpanBuilder.RefType);

        public static Expression New(Expression scriptInfo, Expression closures, Expression @delegate, Expression name, Expression code)
        {
            return Expression.New(_New, scriptInfo, closures, @delegate, name, code);
        }
    }
}
