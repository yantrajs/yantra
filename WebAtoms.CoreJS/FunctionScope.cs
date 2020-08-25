using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebAtoms.CoreJS.Core;

using Exp = System.Linq.Expressions.Expression;

namespace WebAtoms.CoreJS
{
    public class FunctionScope
    {

        public class VariableScope
        {
            public Exp Expression { get; internal set; }
            public string Name { get; internal set; }
        }

        private List<VariableScope> variableScopeList = new List<VariableScope>();

        public FunctionDeclaration Function { get; }

        public ParameterExpression ThisExpression { get; }

        public FunctionScope(Esprima.Ast.FunctionDeclaration fx, ParameterExpression te)
        {
            this.Function = fx;
            this.ThisExpression = te;
        }

        public Exp this[string name]
        {
            get
            {
                return variableScopeList.FirstOrDefault(x => x.Name == name)?.Expression;
            }
        }

        public IDisposable AddVariable(string name, Exp exp)
        {
            var v = new VariableScope
            {
                Name = name,
                Expression = exp
            };
            this.variableScopeList.Add(v);
            return new DisposableAction(() =>
            {
                this.variableScopeList.Remove(v);
            });
        }


    }
}
