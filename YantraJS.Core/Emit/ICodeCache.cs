#if !WEBATOMS
// using FastExpressionCompiler;
#endif
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;


namespace YantraJS.Emit
{

    public delegate YExpression<JSFunctionDelegate> JSCodeCompiler();

    public interface ICodeCache
    {

        JSFunctionDelegate GetOrCreate(in JSCode code);


    }
}
