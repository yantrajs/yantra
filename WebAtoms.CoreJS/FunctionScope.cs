using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using WebAtoms.CoreJS.Core;

using Exp = System.Linq.Expressions.Expression;

namespace WebAtoms.CoreJS
{
    public class LoopScope: LinkedStackItem<LoopScope>
    {
        public readonly LabelTarget Break;
        public readonly LabelTarget Continue;

        public readonly bool IsSwitch;

        public LoopScope(
            LabelTarget breakTarget, 
            LabelTarget continueTarget,
            bool isSwitch = false)
        {
            this.Break = breakTarget;
            this.Continue = continueTarget;
            this.IsSwitch = isSwitch;
        }
    }

    public class FunctionScope: LinkedStackItem<FunctionScope>
    {        

        public class VariableScope
        {
            public ParameterExpression Variable { get; internal set; }
            public Exp Expression { get; internal set; }
            public string Name { get; internal set; }

            public bool Create { get; internal set; }

            public Exp Init { get; internal set; }
        }

        private List<VariableScope> variableScopeList = new List<VariableScope>();

        public Esprima.Ast.IFunction Function { get; }

        public ParameterExpression ThisExpression { get; internal set; }

        public ParameterExpression ArgumentsExpression { get; }

        public ParameterExpression Scope { get; }

        public bool IsRoot => Function == null;

        public LinkedStack<LoopScope> Loop
            = new LinkedStack<LoopScope>();

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

        public FunctionScope(Esprima.Ast.IFunction fx)
        {
            this.Function = fx;
            this.ThisExpression = Expression.Parameter(typeof(Core.JSValue),"_this");
            this.ArgumentsExpression = Expression.Parameter(typeof(Core.JSValue[]),"_arguments");
            this.Scope = Expression.Parameter(typeof(Core.LexicalScope), "lexicalScope");
            ReturnLabel = Expression.Label(typeof(Core.JSValue));
        }

        public Exp this[string name]
        {
            get
            {
                // go up..

                return 
                    variableScopeList.FirstOrDefault(x => x.Name == name)?.Expression
                    ?? (this.Parent?[name]);
            }
        }

        public VariableScope CreateVariable(
            string name,
            Exp exp,
            ParameterExpression pe = null,
            Exp init = null)
        {
            var v = new VariableScope
            {
                Name = name,
                Expression = exp,
                Variable = pe,
                Init = init,
                Create = true
            };
            this.variableScopeList.Add(v);
            return v;
        }


        public VariableScope AddVariable(
            string name, 
            Exp exp, 
            ParameterExpression pe = null,
            Exp init = null)
        {
            var v = new VariableScope
            {
                Name = name,
                Expression = exp,
                Variable = pe,
                Init = init
            };
            this.variableScopeList.Add(v);
            return v;
        }
        public VariableScope GetVariable(string name)
        {
            return this.variableScopeList.FirstOrDefault(x => x.Name == name)
                 ??  this.Parent?.GetVariable(name);
        }


    }
}
