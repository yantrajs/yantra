using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using WebAtoms.CoreJS.Core;

using Exp = System.Linq.Expressions.Expression;

namespace WebAtoms.CoreJS
{
    public class FunctionScope
    {

        public class VariableScope
        {
            public ParameterExpression Variable { get; internal set; }
            public Exp Expression { get; internal set; }
            public string Name { get; internal set; }
        }

        private List<VariableScope> variableScopeList = new List<VariableScope>();

        public Esprima.Ast.FunctionDeclaration Function { get; }

        public ParameterExpression ThisExpression { get; }

        public ParameterExpression ArgumentsExpression { get; }

        public bool IsRoot => Function == null;

        public IEnumerable<VariableScope> Variables
        {
            get
            {
                foreach(var s in variableScopeList)
                {
                    if (s.Variable != null)
                    {
                        yield return s;
                    }
                }
            }
        }

        public LabelTarget ReturnLabel { get; }

        public FunctionScope(
            Esprima.Ast.FunctionDeclaration fx)
        {
            this.Function = fx;
            this.ThisExpression = Expression.Parameter(typeof(JSValue));
            this.ArgumentsExpression = Expression.Parameter(typeof(JSArray));
            ReturnLabel = Expression.Label(typeof(JSValue));
        }

        public Exp this[string name]
        {
            get
            {
                return variableScopeList.FirstOrDefault(x => x.Name == name)?.Expression;
            }
        }

        public IDisposable AddVariable(string name, Exp exp, ParameterExpression pe = null)
        {
            var v = new VariableScope
            {
                Name = name,
                Expression = exp,
                Variable = pe
            };
            this.variableScopeList.Add(v);
            return new DisposableAction(() =>
            {
                this.variableScopeList.Remove(v);
            });
        }


    }
}
