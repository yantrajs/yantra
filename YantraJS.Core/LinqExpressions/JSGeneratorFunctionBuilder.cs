using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;

namespace YantraJS.ExpHelper
{
    public class JSAsyncFunctionBuilder
    {
        private static Type type = typeof(JSAsyncFunction);

        private static ConstructorInfo _New =
            type.Constructor(typeof(ScriptInfo), typeof(JSVariable[]), typeof(JSAsyncDelegate), StringSpanBuilder.RefType, StringSpanBuilder.RefType);

        public static Expression New(Expression scriptInfo, Expression closures, Expression @delegate, Expression name, Expression code)
        {
            return Expression.New(_New, scriptInfo, closures, @delegate, name, code);
        }
    }


    public class JSGeneratorFunctionBuilder
    {
        private static Type type = typeof(JSGeneratorFunction);

        private static ConstructorInfo _New =
            type.Constructor(
                typeof(ScriptInfo),
                typeof(JSVariable[]), typeof(JSGeneratorDelegate), StringSpanBuilder.RefType, StringSpanBuilder.RefType);

        public static Expression New(Expression scriptInfo, Expression closures, Expression @delegate, Expression name, Expression code)
        {
            return Expression.New(_New, scriptInfo, closures, @delegate, name, code);
        }
    }
}
