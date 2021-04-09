using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        public Expression KeyOfName(string name)
        {
            // search for variable...
            if (KeyStringsBuilder.Fields.TryGetValue(name, out var fx))
                return fx;

            var i = _keyStrings.GetOrAdd(name);
            return ScriptInfoBuilder.KeyString(this.scope.Top.ScriptInfo, (int)i);
        }

        public Expression KeyOfName(in StringSpan name)
        {
            // search for variable...
            if (KeyStringsBuilder.Fields.TryGetValue(name, out var fx))
                return fx;

            var i = _keyStrings.GetOrAdd(name);
            return ScriptInfoBuilder.KeyString(this.scope.Top.ScriptInfo, (int)i);
        }
    }
}
