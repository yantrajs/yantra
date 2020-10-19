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

    public class AstPair<TAst, TExp> 
        where TAst: Esprima.Ast.INode
        where TExp: Exp
    {
        public TAst Ast;
        public TExp Init;
        public AstPair(TAst ast, TExp init)
        {
            this.Ast = ast;
            this.Init = init;
        }

        public AstPair(TAst ast)
        {
            this.Ast = ast;
        }

    }

    public class FunctionScope: LinkedStackItem<FunctionScope>
    {

        public AstPair<Esprima.Ast.VariableDeclaration, Exp> PushToNewScope { get; set; }

        public class VariableScope: IDisposable
        {
            public ParameterExpression Variable { get; internal set; }
            public Exp Expression { get; internal set; }
            public string Name { get; internal set; }

            public bool Create { get; internal set; }

            public Exp Init { get; private set; }

            public bool InUse { get; internal set; }

            public void Dispose()
            {
                InUse = false;
            }

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
                    if (exp != null)
                    {
                        Init = Exp.Assign(Variable, exp);
                    }
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

        public Expression Super { get; private set; }

        public FunctionScope(Esprima.Ast.IFunction fx, Expression previousThis = null, Expression super = null)
        {
            this.Function = fx;
            if (fx?.Generator ?? false)
            {
                Generator = Expression.Parameter(typeof(JSWeakGenerator).MakeByRefType());
            } else
            {
                Generator = null;
            }
            this.Super = super;
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
            TempVariables = new List<VariableScope>();
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
            this.Super = p.Super;
            this.TempVariables = p.TempVariables;
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

        private List<VariableScope> TempVariables;

        public VariableScope GetTempVariable(Type type = null)
        {
            type = type ?? typeof(JSValue);
            var ts = TempVariables.FirstOrDefault(x => x.Variable.Type == type && x.InUse == false);
            if(ts == null) { 
                var t = Expression.Variable(type);
                ts = new VariableScope
                {
                    Name = "#temp",
                    Variable = t,
                    Expression = t,
                    Create = true
                };
                TempVariables.Add(ts);
                TopScope.variableScopeList.Add(ts);
            }
            ts.InUse = true;
            return ts;
        }

        public VariableScope CreateVariable(
            string name,
            Exp init = null,
            bool newScope = false,
            Type type = null)
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

            var pe = Expression.Parameter(type ?? typeof(JSVariable), name);
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
