#if !WEBATOMS
// using FastExpressionCompiler;
#endif
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Emit
{

    public delegate Expression<JSFunctionDelegate> JSCodeCompiler();

    public interface ICodeCache
    {

        JSFunctionDelegate GetOrCreate(in JSCode code);


    }
}
