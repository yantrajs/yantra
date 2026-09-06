using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.ExpHelper;
using YantraJS.Expressions;

namespace YantraJS.Core.LambdaGen;

internal static class JSValueGen
{


    public static YExpression InvokeJSMethod(this YExpression target, KeyString method)
    {
        return NewLambdaExpression
            .StaticCallExpression(() =>
            () => JSValueExtensions.InvokeMethod(null, method),
            target,
            YExpression.Constant((uint)method)
            );

    }


}
