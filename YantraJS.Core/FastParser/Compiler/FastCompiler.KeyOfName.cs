using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        public Expression KeyOfName(string name)
        {
            // search for variable...
            if (Enum.TryParse<KeyString>(name, out var v))
                return Expression.Constant((int)v, typeof(KeyString));

            // var i = _KeyString.GetOrAdd(name);
            // return ScriptInfoBuilder.KeyString(this.scriptInfo, (int)i);
            var key = KeyStrings.Instance.GetOrCreate(name);
            return Expression.Constant((int)key, typeof(KeyString));
        }

        public Expression KeyOfName(in StringSpan name)
        {
            // search for variable...
            if (Enum.TryParse<KeyString>(name.Value, out var v))
                return Expression.Constant((int)v, typeof(KeyString));

            //var i = _KeyString.GetOrAdd(name);
            //return ScriptInfoBuilder.KeyString(this.scriptInfo, (int)i);
            var key = KeyStrings.Instance.GetOrCreate(name);
            return Expression.Constant((int)key, typeof(KeyString));
        }
    }
}
