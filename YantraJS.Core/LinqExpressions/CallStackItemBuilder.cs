using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core.CodeGen;
using YantraJS.Expressions;

namespace YantraJS.Core.LinqExpressions
{
    internal static class CallStackItemBuilder
    {

        static Type type = typeof(CallStackItem);

        static ConstructorInfo _new =
            type.GetConstructor(new Type[] { 
                typeof(JSContext),
                typeof(ScriptInfo),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(int)
            });

        static MethodInfo _step =
            type.PublicMethod(nameof(CallStackItem.Step), typeof(int), typeof(int));

        public static YExpression New(
            YExpression context,
            YExpression scriptInfo,
            int nameOffset,
            int nameLength,
            int line,
            int column)
        {
            return YExpression.New(
                _new, 
                context, 
                scriptInfo, 
                YExpression.Constant(nameOffset),
                YExpression.Constant(nameLength),
                YExpression.Constant(line),
                YExpression.Constant(column));
        }

        public static YExpression Step(YExpression target, int line, int column)
        {
            return YExpression.Call(target,
                _step,
                YExpression.Constant(line),
                YExpression.Constant(column));
        }

    }
}
