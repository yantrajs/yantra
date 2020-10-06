using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Generator;
using WebAtoms.CoreJS.ExpHelper;
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

            public Exp Init { get; private set; }

            public void SetInit(Expression exp)
            {
                if (Variable.Type == typeof(JSVariable))
                {
                    if (exp != null)
                    {
                        if (typeof(JSValue).IsAssignableFrom(exp.Type))
                        {
                            Init = Exp.Assign(Variable, JSVariableBuilder.New(exp, Name));
                        }
                        else
                        {
                            Init = Exp.Assign(Variable, exp);
                        }
                    }
                    else
                    {
                        Init = Exp.Assign(Variable, JSVariableBuilder.New(Name));
                    }
                }
                else
                {
                    Init = Exp.Assign(Variable, exp);
                }
            }
        }

        private List<VariableScope> variableScopeList = new List<VariableScope>();

        public Esprima.Ast.IFunction Function { get; }

        public Expression ThisExpression { get; }

        public Expression ArgumentsExpression { get; }

        public ParameterExpression Arguments { get; }

        public ParameterExpression Scope { get; }

        public bool IsRoot => Function == null;

        public LinkedStack<LoopScope> Loop;

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

        public IEnumerable<ParameterExpression> VariableParameters
        {
            get
            {
                foreach (var s in variableScopeList)
                {
                    if (s.Variable != null)
                    {
                        yield return s.Variable;
                    }
                }
            }
        }

        public IEnumerable<Expression> InitList
        {
            get
            {
                foreach (var s in variableScopeList)
                {
                    if (s.Init != null)
                    {
                        yield return s.Init;
                    }
                }
            }
        }


        public LabelTarget ReturnLabel { get; }

        public FunctionScope TopScope
        {
            get
            {
                var p = this;
                while (p.Parent != null && p.Function == p.Parent.Function)
                {
                    p = p.Parent;
                }
                return p;
            }
        }

        public ParameterExpression Generator
        {
            get;set;
        }

        public FunctionScope(Esprima.Ast.IFunction fx, Expression previousThis = null)
        {
            this.Function = fx;
            if (fx?.Generator ?? false)
            {
                Generator = Expression.Parameter(typeof(JSGenerator));
            } else
            {
                Generator = null;
            }
            // this.ThisExpression = Expression.Parameter(typeof(Core.JSValue),"_this");
            // this.ArgumentsExpression = Expression.Parameter(typeof(Core.JSValue[]),"_arguments");
            this.Arguments = Expression.Parameter(typeof(Arguments).MakeByRefType());
            this.ArgumentsExpression = Arguments;
            if (previousThis != null)
            {
                this.ThisExpression = previousThis;
            }
            else
            {
                // this.ThisExpression = Expression.Parameter(typeof(JSValue));
                var _this = this.CreateVariable("this", ArgumentsBuilder.This(Arguments) );
                this.ThisExpression = _this.Expression;
            }

            this.Scope = Expression.Parameter(typeof(Core.LexicalScope), "lexicalScope");
            this.Loop = new LinkedStack<LoopScope>();
            ReturnLabel = Expression.Label(typeof(Core.JSValue));
        }

        public FunctionScope(
            FunctionScope p
            )
        {
            this.Function = p.Function;
            this.ThisExpression = p.ThisExpression;
            this.ArgumentsExpression = p.ArgumentsExpression;
            this.Generator = p.Generator;
            this.Scope = Expression.Parameter(typeof(Core.LexicalScope), "lexicalScope");
            this.Loop = p.Loop;
            ReturnLabel = p.ReturnLabel;
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

        public VariableScope CreateException(string name)
        {
            var v = this.variableScopeList.FirstOrDefault(x => x.Name == name);
            if (v != null)
                return v;
            v = new VariableScope {
                Variable = Exp.Parameter(typeof(Exception), name + "Exp")
            };
            this.variableScopeList.Add(v);
            
            return v;
        }

        private VariableScope tempVariable;

        public VariableScope GetTempVariable()
        {
            var ts = TopScope;
            if(ts.tempVariable == null)
            {
                var t = Expression.Variable(typeof(JSValue));
                ts.variableScopeList.Add(new VariableScope { 
                    Name = "#temp",
                    Variable = t,
                    Expression = t,
                    Create = true
                });
            }
            return ts.tempVariable;
        }

        public VariableScope CreateVariable(
            string name,
            Exp init = null,
            bool newScope = false)
        {
            var v = this.variableScopeList.FirstOrDefault(x => x.Name == name);
            if (v != null)
                return v;

            // search parent if it is in same function scope...
            if (!newScope)
            {
                var p = this.Parent;
                while (p != null && p.Function == this.Function)
                {
                    v = p.variableScopeList.FirstOrDefault(x => x.Name == name);
                    if (v != null)
                        return v;
                    p = p.Parent;
                }
            }

            var pe = Expression.Parameter(typeof(JSVariable), name);
            var ve = JSVariable.ValueExpression(pe);
            v = new VariableScope
            {
                Name = name,
                Expression = ve,
                Variable = pe,
                Create = true
            };
            v.SetInit(init);
            this.variableScopeList.Add(v);
            return v;
        }


        //public VariableScope AddVariable(
        //    string name, 
        //    Exp exp, 
        //    ParameterExpression pe = null,
        //    Exp init = null)
        //{
        //    var v = new VariableScope
        //    {
        //        Name = name,
        //        Expression = exp,
        //        Variable = pe,
        //        Init = init
        //    };
        //    this.variableScopeList.Add(v);
        //    return v;
        //}
        public VariableScope GetVariable(string name)
        {
            return this.variableScopeList.FirstOrDefault(x => x.Name == name)
                 ??  this.Parent?.GetVariable(name);
        }


    }
}
