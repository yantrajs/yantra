using FastExpressionCompiler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Emit
{
    public readonly struct JSCode
    {
        public readonly string Location;

        public readonly StringSpan Code;

        public readonly IList<string> Arguments;

        public readonly JSCodeCompiler Compiler;

        public JSCode Clone()
        {
            return new JSCode(Location, Code, Arguments, Compiler);
        }

        public string Key
        {
            get
            {
                if (Arguments != null)
                {
                    return $"`ARGS:{(string.Join(",", Arguments))}\r\n{Code}";
                }
                return $"`ARGS:\r\n{Code}";
            }
        }

        public JSCode(string location, in StringSpan code, IList<string> args, JSCodeCompiler compiler)
        {
            this.Location = location;
            this.Code = code;
            this.Arguments = args;
            this.Compiler = compiler;
        }
    }

    public delegate Expression<JSFunctionDelegate> JSCodeCompiler();

    public interface ICodeCache
    {

        JSFunctionDelegate GetOrCreate(in JSCode code);


    }

    public class DictionaryCodeCache : ICodeCache
    {
        private static ConcurrentStringMap<JSFunctionDelegate> cache
            = ConcurrentStringMap<JSFunctionDelegate>.Create();

        public static ICodeCache Current = new DictionaryCodeCache();

        public JSFunctionDelegate GetOrCreate(in JSCode code)
        {
            var compiler = code.Compiler;
            return cache.GetOrCreate(code.Key, (k) => {
                var  exp = compiler();
                return exp.Compile();
            });
        }

    }
}
