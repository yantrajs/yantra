using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions.Generators;
using YantraJS.Core.LinqExpressions.GeneratorsV2;
using YantraJS.ExpHelper;
using YantraJS.Utils;
using Exp = System.Linq.Expressions.Expression;
namespace YantraJS.Core.FastParser.Compiler
{
    public class FastFunctionScope : LinkedStackItem<FastFunctionScope>
    {

        public ScopedVariableDeclaration PushToNewScope { get; set; }

        public class VariableScope : IDisposable
        {
            public ParameterExpression Variable { get; internal set; }
            public Exp Expression { get; internal set; }
            public string Name { get; internal set; }

            public bool Create { get; internal set; }

            public Expression Init { get; private set; }

            /// <summary>
            /// Create Variable first and then assign it, in next step.
            /// 
            /// This is required for recursive function as name/instance of function
            /// is null when it is being created and accessed at the same time
            /// </summary>
            public Expression PostInit { get; private set; }

            public bool InUse { get; internal set; }
            public bool IsTemp { get; internal set; }

            public void Dispose()
            {
                InUse = false;
            }

            public void SetPostInit(Expression exp)
            {
                if (exp == null)
                {
                    PostInit = null;
                    return;
                }
                if (Variable.Type == typeof(JSVariable))
                {
                    if (exp.Type == typeof(JSVariable))
                    {
                        PostInit = Exp.Assign(Variable, exp);
                        return;
                    }
                }
                PostInit = Exp.Assign(Expression, exp);
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

        private SharedParserStringMap<VariableScope> variableScopeList = new SharedParserStringMap<VariableScope>();

        public AstFunctionExpression Function { get; }

        private Expression _this;
        public Expression ThisExpression => _this ?? (_this = this.GetVariable("this", true).Expression);

        public Expression ArgumentsExpression { get; }

        public ParameterExpression Arguments { get; }

        public ParameterExpression Context { get; }

        public ParameterExpression StackItem { get; }

        public ParameterExpression Closures { get; }

        public ParameterExpression ScriptInfo { get; }

        public bool IsRoot => Function == null;

        public LinkedStack<LoopScope> Loop;

        public IEnumerable<VariableScope> Variables
        {
            get
            {
                var en = variableScopeList.AllValues;
                while (en.MoveNext(out var s))
                {
                    if (s.Value.Variable != null)
                    {
                        yield return s.Value;
                    }
                }
            }
        }

        public IEnumerable<ParameterExpression> VariableParameters
        {
            get
            {
                var en = variableScopeList.AllValues;
                while (en.MoveNext(out var s))
                {
                    if (s.Value.Variable != null)
                    {
                        yield return s.Value.Variable;
                    }
                }
            }
        }

        public IEnumerable<Expression> InitList
        {
            get
            {
                var en = variableScopeList.AllValues;
                while (en.MoveNext(out var s))
                {
                    if (s.Value.Init != null)
                    {
                        yield return s.Value.Init;
                    }
                }
                en = variableScopeList.AllValues;
                while (en.MoveNext(out var s))
                {
                    if (s.Value.PostInit != null)
                    {
                        yield return s.Value.PostInit;
                    }
                }
            }
        }




        public LabelTarget ReturnLabel { get; }

        public readonly FastFunctionScope TopScope;

        //public FastFunctionScope TopScope
        //{
        //    get
        //    {
        //        var p = this;
        //        while (p.Parent != null && p.Function == p.Parent.Function)
        //        {
        //            p = p.Parent;
        //        }
        //        return p;
        //    }
        //}

        //public FunctionScope TopStackScope
        //{
        //    get
        //    {
        //        var p = this;
        //        if (p.variableScopeList.Any())
        //            return p;
        //        while (p.Parent != null && p.Function == p.Parent.Function)
        //        {
        //            p = p.Parent;
        //            if (p.variableScopeList.Any())
        //                return p;
        //        }
        //        return p;
        //    }
        //}

        public ParameterExpression Generator
        {
            get; set;
        }

        public ParameterExpression Awaiter { get; set; }

        public Expression Super { get; private set; }

        private static int scopeID = 0;

        public FastFunctionScope(
            FastPool pool,
            AstFunctionExpression fx, Expression previousThis = null, Expression super = null)
        {
            TopScope = this;
            var sID = Interlocked.Increment(ref scopeID);
            this.pool = pool.NewScope();
            this.Function = fx;
            if (fx?.Generator ?? false)
            {
                Generator = Expression.Parameter(typeof(ClrGeneratorV2), "clrGenerator");
            }
            else
            {
                Generator = null;
            }
            if (fx?.Async ?? false)
            {
                Awaiter = Expression.Parameter(typeof(JSWeakAwaiter).MakeByRefType());
            }
            this.Super = super;
            // this.ThisExpression = Expression.Parameter(typeof(Core.JSValue),"_this");
            // this.ArgumentsExpression = Expression.Parameter(typeof(Core.JSValue[]),"_arguments");
            this.Arguments = (fx?.Generator ?? false)
                ? Expression.Parameter(typeof(Arguments), $"a-{sID}")
                : Expression.Parameter(typeof(Arguments).MakeByRefType(), $"a-{sID}");
            this.ArgumentsExpression = Arguments;
            if (previousThis != null)
            {
                // this.ThisExpression = previousThis;
            }
            else
            {
                // this.ThisExpression = Expression.Parameter(typeof(JSValue));

                // this is needed to fix closure over lambda
                // this can be improved
                var t = this.CreateVariable("this", ArgumentsBuilder.This(Arguments));
                _this = t.Expression;
                // this.ThisExpression = _this.Expression;
            }

            this.Context = Expression.Parameter(typeof(JSContext), $"{nameof(Context)}{sID}");
            this.StackItem = Expression.Parameter(typeof(CallStackItem), $"{nameof(StackItem)}{sID}");
            this.Closures = Expression.Parameter(typeof(JSVariable[]), $"{nameof(Closures)}{sID}");
            this.ScriptInfo = Expression.Parameter(typeof(ScriptInfo), $"{nameof(ScriptInfo)}{sID}");
            this.Loop = new LinkedStack<LoopScope>();
            TempVariables = pool.AllocateList<VariableScope>();
            ReturnLabel = Expression.Label(typeof(YantraJS.Core.JSValue));
        }

        public FastFunctionScope(
            FastFunctionScope p
            )
        {
            this.Function = p.Function;
            this.TopScope = p.TopScope;
            this.pool = p.pool.NewScope();
            // this.ThisExpression = p.ThisExpression;
            this.ArgumentsExpression = p.ArgumentsExpression;
            this.Generator = p.Generator;
            this.Awaiter = p.Awaiter;
            this.Super = p.Super;
            this.TempVariables = p.TempVariables;
            // this.Scope = Expression.Parameter(typeof(Core.LexicalScope), "lexicalScope");
            this.Context = p.Context;
            this.StackItem = p.StackItem;
            this.Closures = p.Closures;
            this.ScriptInfo = p.ScriptInfo;
            this.Loop = p.Loop;
            ReturnLabel = p.ReturnLabel;
        }

        public Exp this[string name]
        {
            get
            {
                return GetVariable(name).Expression;
            }
        }

        public VariableScope CreateException(string name)
        {
            var v = new VariableScope
            {
                Variable = Exp.Parameter(typeof(Exception), name + "Exp")
            };
            this.variableScopeList[name + DateTime.UtcNow.Ticks] = v;
            v.Expression = v.Variable;
            return v;
        }

        private FastList<VariableScope> TempVariables;
        private readonly FastPool.Scope pool;

        private int id;

        public VariableScope GetTempVariable(Type type = null)
        {
            type = type ?? typeof(JSValue);
            var fe = TopScope.variableScopeList.AllValues;
            while (fe.MoveNext(out var item))
            {
                var v = item.Value;
                if (v.IsTemp && v.Expression.Type == type && !v.InUse)
                {
                    v.InUse = true;
                    return v;
                }
            }
            var tp = Exp.Variable(type, "#Temp" + type.Name + TopScope.id++);
            var temp = new VariableScope {
                Create = true,
                Name = tp.Name,
                IsTemp = true,
                Expression = tp,
                Variable = tp
            };
            TopScope.variableScopeList[temp.Name] = temp;
            return temp;
        }

        public bool IsFunctionScope => this.Parent?.Function != this.Function;

        public VariableScope CreateVariable(
            in StringSpan name,
            Exp init = null,
            bool newScope = false,
            Type type = null)
        {
            var v = this.variableScopeList[name];
            if (v != null)
                return v;

            // search parent if it is in same function scope...
            if (!newScope)
            {
                var p = this.Parent;
                while (p != null && p.Function == this.Function)
                {
                    v = p.variableScopeList[name];
                    if (v != null)
                        return v;
                    p = p.Parent;
                }
            }

            // we need to move variable in top scope...
            var pe = Expression.Parameter(type ?? typeof(JSVariable), name.Value);
            var ve = JSVariable.ValueExpression(pe);
            v = new VariableScope
            {
                Name = name.Value,
                Expression = ve,
                Variable = pe,
                Create = true
            };
            v.SetInit(init);
            this.variableScopeList[name] = v;
            return v;
        }


        public override void Dispose()
        {
            base.Dispose();
            pool.Dispose();
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

        public FastList<VariableScope> ClosureList
        {
            get; private set;
        }

        public VariableScope GetVariable(in StringSpan name, bool createClosure = true)
        {

            var start = this;
            while (true)
            {

                if (start.variableScopeList.TryGetValue(name, out var result))
                {
                    return result;
                }
                if (start.Parent == null)
                    return null;
                if (start.Parent.Function != start.Function)
                    break;
                start = start.Parent;
            }

            if (!createClosure)
                throw new ArgumentOutOfRangeException($"{name} not found in current variable scope");

            return start.CreateClosure(name);
        }

        private VariableScope CreateClosure(in StringSpan name)
        {
            var p = Parent;
            if (p == null)
                return null;
            var v = p.GetVariable(name);
            if (v == null)
                return null;
            ClosureList = ClosureList ?? pool.AllocateList<VariableScope>();
            var v1 = new VariableScope()
            {
                Variable = Expression.Parameter(typeof(JSVariable), name.Value),
                Name = name.Value
            };
            int index = ClosureList.Count;
            v1.Expression = JSVariable.ValueExpression(v1.Variable);
            v1.SetInit(Expression.ArrayIndex(Closures, Expression.Constant(index)));
            ClosureList.Add(v);
            this.variableScopeList[name] = v1;
            return v1;
        }


    }
}
