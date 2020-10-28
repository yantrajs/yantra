using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Emit
{
    public struct JSCode
    {
        public readonly string Location;

        public readonly string Code;

        public readonly IList<string> Arguments;

        public JSCode Clone()
        {
            return new JSCode(Location, Code, Arguments);
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

        public JSCode(string location, string code, IList<string> args)
        {
            this.Location = location;
            this.Code = code;
            this.Arguments = args;
        }
    }

    public delegate JSFunctionDelegate JSCodeCompiler(in JSCode code);

    public interface ICodeCache
    {

        JSFunctionDelegate GetOrCreate(in JSCode code, 
            JSCodeCompiler compiler);

        void Save(string location, Expression<JSFunctionDelegate> expression);

    }

    public class DictionaryCodeCache : ICodeCache
    {
        private static ConcurrentDictionary<string, JSFunctionDelegate> cache
            = new ConcurrentDictionary<string, JSFunctionDelegate>();

        public static ICodeCache Current = new DictionaryCodeCache();

        public JSFunctionDelegate GetOrCreate(in JSCode code, JSCodeCompiler compiler)
        {
            var c = code.Code;
            var location = code.Location;
            var args = code.Arguments;
            return cache.GetOrAdd(code.Key, (k) => {
                var a = new JSCode(location, c, args);
                return compiler(a);
            });
        }

        public void Save(string location, Expression<JSFunctionDelegate> expression)
        {
            
        }
    }
}
