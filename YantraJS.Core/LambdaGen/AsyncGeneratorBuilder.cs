using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.ExpHelper;
using YantraJS.Expressions;

namespace YantraJS.Core.LambdaGen;

internal static class AsyncGeneratorBuilder
{

    public static YExpression CheckIfDoneIsTrue(this YExpression target)
    {
        var done = KeyString.done;
        var r = target.MakeIndexExpression<JSValue, KeyString, JSValue>(
            () => (x, k) => x[k],
            YExpression.Constant(done));
        return JSValueBuilder.BooleanValue(r);
    }

    public static YExpression ValueProperty(this YExpression target)
    {
        return target.MakeIndexExpression<JSValue,KeyString,JSValue>(() => (x, k) => x[k],
            YExpression.Constant(KeyString.value));
    }

}
