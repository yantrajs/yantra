using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core.CodeGen;
using YantraJS.Core.LambdaGen;
using YantraJS.Expressions;

namespace YantraJS.Core.LinqExpressions
{
    internal static class CallStackItemBuilder
    {

        public static YExpression New(
            YExpression context,
            YExpression scriptInfo,
            int nameOffset,
            int nameLength,
            int line,
            int column)
        {

            return NewLambdaExpression.NewExpression<CallStackItem>(() => () => new CallStackItem(
                (JSContext)null,
                (ScriptInfo)null, 0, 0, 0, 0),
                context,
                scriptInfo,
                YExpression.Constant(nameOffset),
                YExpression.Constant(nameLength),
                YExpression.Constant(line),
                YExpression.Constant(column));
        }

        public static YExpression Step(YExpression target, int line, int column)
        {
            return target.CallExpression<CallStackItem, int, int>(() => (x, a, b) => x.Step(a, b),
                YExpression.Constant(line),
                YExpression.Constant(column)
                );
        }

    }
}
