using System;
using System.Linq;
using System.Linq.Expressions;
using WebAtoms.CoreJS.Core;

using Exp = System.Linq.Expressions.Expression;

namespace WebAtoms.CoreJS
{
    /// <summary>
    /// Convert variable to named to variable reference when a variable or
    /// parameter is captured.
    /// 
    /// 1. First store everything as ParameterExpression
    /// 2. On capture, convert ParameterExpression to FieldExpression of ParameterExpression
    /// 3. Change all references.. (complicated)
    /// 
    /// Is it possible to create a event listener to add change of variable...?
    /// </summary>
    public class CodeLexicalScope
    {
        private BinaryUInt32Map<ParameterExpression> map = new BinaryUInt32Map<ParameterExpression>();
        readonly CodeLexicalScope parent;

        public CodeLexicalScope(CodeLexicalScope parent)
        {
            this.parent = parent;
        }
        public ParameterExpression Push(Type type, string name)
        {
            var pe = Exp.Parameter(type, name);
            KeyString k = name;
            map[k.Key] = pe;
            return pe;
        }

        public Exp Search(string name)
        {
            KeyString k = name;
            if (map.TryGetValue(k.Key, out var pe))
                return pe;
            if (parent != null)
            {
                return parent.Search(name);
            }
            // need to call JSContext.Current[name];
            return null;
        }


    }
}
