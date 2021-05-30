using Esprima;
using Esprima.Ast;
// using FastExpressionCompiler;
using Microsoft.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions;
using YantraJS.Core.LinqExpressions.Generators;
using YantraJS.Core.LinqExpressions.Logical;
using YantraJS.Emit;
using YantraJS.ExpHelper;
using YantraJS.Extensions;
using YantraJS.LinqExpressions;
using YantraJS.Parser;
using YantraJS.Utils;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
namespace YantraJS
{

    public class CoreScript
    {
        internal static JSFunctionDelegate Compile(in StringSpan code, string location = null, IList<string> args = null, ICodeCache codeCache = null)
        {
            try
            {
                codeCache = codeCache ?? DictionaryCodeCache.Current;
                var script = code;
                var jsc = new JSCode(location, code, args, () =>
                {
                    var cc = new Core.FastParser.Compiler.FastCompiler(script, location, args, codeCache);
                    return cc.Method;
                });
                return codeCache.GetOrCreate(in jsc);
            }
            catch (Core.FastParser.FastParseException ex)
            {
                throw new JSException(ex.Message, JSContext.Current.SyntaxErrorPrototype, "Compile", location, ex.Token.Start.Line);
            }
        }

        public static JSValue EvaluateWithTasks(string code, string location = null)
        {
            var fx = Compile(code, location);
            var result = JSUndefined.Value;
            var ctx = JSContext.Current;
            AsyncPump.Run(() =>
            {
                result = fx(new Arguments(ctx));
                return Task.CompletedTask;
            });
            return result;
        }


        public static JSValue Evaluate(string code, string location = null, ICodeCache codeCache = null)
        {
            var fx = Compile(code, location, null, codeCache);
            var result = JSUndefined.Value;
            var ctx = JSContext.Current;
            result = fx(new Arguments(ctx));
            return result;
        }

    }

    public class ExpressionHolder
    {
        public bool Static;
        public Exp Key;
        public Exp Value;
        public Exp Getter;
        public Exp Setter;
    }

}
