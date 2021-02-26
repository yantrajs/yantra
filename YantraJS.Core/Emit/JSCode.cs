#if !WEBATOMS
// using FastExpressionCompiler;
#endif
using System.Collections.Generic;
using YantraJS.Core;

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
}
