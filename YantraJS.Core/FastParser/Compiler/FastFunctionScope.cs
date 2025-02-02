using System;
using System.Collections.Generic;
using System.Threading;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Generator;
using YantraJS.Core.LinqExpressions.GeneratorsV2;
using YantraJS.ExpHelper;
using YantraJS.Utils;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using YantraJS.Core.Core.Storage;
using YantraJS.Expressions;
using YantraJS.Core.Core.Disposable;

namespace YantraJS.Core.FastParser.Compiler
{

    public class SharedParserStringMap<T>
    {
        private static ConcurrentNameMap parserStringCache = new ConcurrentNameMap();
        private SAUint32Map<uint> indexes;
        private (StringSpan Key, T Value)[] storage;

        private uint length;

        public T this[in StringSpan name]
        {
            get
            {
                if (parserStringCache.TryGetValue(in name, out var id))
                {
                    if (indexes.TryGetValue(id.Key, out var index))
                        return storage[index].Value;
                }
                return default;
            }
            set
            {
                var a = parserStringCache.Get(in name);
                if (!indexes.TryGetValue(a.Key, out var id))
                {
                    id = length++;
                    // indexes[a.Key] = id;
                    indexes.Put(a.Key) = id;
                }
                Save(id, in name, in value);
            }
        }

        private void Save(uint index, in StringSpan key, in T value)
        {
            storage = storage ?? (new (StringSpan, T)[8]);
            if (index >= storage.Length)
            {
                Array.Resize(ref storage, (((int)index >> 2) + 1) << 2);
            }
            storage[index] = (key, value);
        }

        public bool TryGetValue(in StringSpan name, out T value)
        {
            if (parserStringCache.TryGetValue(in name, out var id))
            {
                if (indexes.TryGetValue(id.Key, out var index))
                {
                    value = storage[index].Value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public IFastEnumerator<(StringSpan Key, T Value)> AllValues => new Enumerator(this);

        private class Enumerator : IFastEnumerator<(StringSpan Key, T Value)>
        {
            private SharedParserStringMap<T> map;
            private int index = -1;

            public Enumerator(SharedParserStringMap<T> map)
            {
                this.map = map;
            }

            public bool MoveNext(out (StringSpan Key, T Value) item)
            {
                while (true)
                {
                    this.index++;
                    if (this.index < map.length)
                    {
                        item = map.storage[index];
                        return true;
                    }
                    else break;
                }
                item = (StringSpan.Empty, default);
                return false;
            }

            public bool MoveNext(out (StringSpan Key, T Value) item, out int index)
            {
                while (true)
                {
                    this.index++;
                    if (this.index < map.length)
                    {
                        index = this.index;
                        item = map.storage[index];
                        return true;
                    }
                    else break;
                }
                item = (StringSpan.Empty, default);
                index = 0;
                return false;
            }
        }

    }
    public class FastFunctionScope : LinkedStackItem<FastFunctionScope>
    {

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

        // public Expression NewTarget => Expression.Field(ArgumentsExpression, nameof(YantraJS.Core.Arguments.NewTarget));

        public bool HasDisposable => _dispoable != null;

        private ParameterExpression _dispoable;
        public ParameterExpression Disposable => _dispoable ?? (_dispoable = Expression.Parameter(typeof(JSDisposableStack)));

        public Expression ArgumentsExpression { get; }

        public ParameterExpression Arguments { get; }

        public ParameterExpression Context { get; }

        public ParameterExpression StackItem { get; }

        // public ParameterExpression Closures { get; }

        // public ParameterExpression ScriptInfo { get; }

        public bool IsRoot => Function == null;

        public LinkedStack<LoopScope> Loop;

        // public Expression Super => JSValueBuilder.PrototypeChain( JSValueBuilder.PrototypeChain(ThisExpression));

        public Expression Super { get; set; }

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

        private static int scopeID = 0;

        public IFastEnumerable<AstClassProperty> MemberInits { get; set; }

        public readonly FastFunctionScope RootScope;

        public FastFunctionScope(
            FastPool pool,
            AstFunctionExpression fx, Expression previousThis = null, Expression super = null,
            bool isAsync = false,
            IFastEnumerable<AstClassProperty> memberInits = null,
            FastFunctionScope previous = null)
        {
            this.RootScope = previous ?? this;
            TopScope = this;
            var sID = Interlocked.Increment(ref scopeID);
            this.MemberInits = memberInits;
            this.Function = fx;
            this.Super = super;
            if (fx?.Generator ?? false)
            {
                Generator = Expression.Parameter(typeof(ClrGeneratorV2), "clrGenerator");
            }
            else
            {
                Generator = null;
            }
            if (fx?.Async ?? true)
            {
                Generator = Expression.Parameter(typeof(ClrGeneratorV2), "clrGenerator");
                // Awaiter = Expression.Parameter(typeof(JSWeakAwaiter).MakeByRefType());
                // throw new NotSupportedException();
            }
            if (isAsync && Generator == null)
            {
                Generator = Expression.Parameter(typeof(ClrGeneratorV2), "clrGenerator");
            }
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
            // this.Closures = Expression.Parameter(typeof(JSVariable[]), $"{nameof(Closures)}{sID}");
            // this.ScriptInfo = Expression.Parameter(typeof(ScriptInfo), $"{nameof(ScriptInfo)}{sID}");
            this.Loop = new LinkedStack<LoopScope>();
            TempVariables = new Sequence<VariableScope>();
            ReturnLabel = Expression.Label(typeof(YantraJS.Core.JSValue));
        }

        public FastFunctionScope(
            FastFunctionScope p
            )
        {
            this.Function = p.Function;
            this.TopScope = p.TopScope;
            this.RootScope = p.RootScope;
            this.MemberInits = p.MemberInits;
            // this.pool = p.pool.NewScope();
            // this.ThisExpression = p.ThisExpression;
            this.ArgumentsExpression = p.ArgumentsExpression;
            this.Generator = p.Generator;
            this.Awaiter = p.Awaiter;
            this.TempVariables = p.TempVariables;
            // this.Scope = Expression.Parameter(typeof(Core.LexicalScope), "lexicalScope");
            this.Super = p.Super;
            this.Context = p.Context;
            this.StackItem = p.StackItem;
            // this.Closures = p.Closures;
            // this.ScriptInfo = p.ScriptInfo;
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

        private Sequence<VariableScope> TempVariables;
        // private readonly FastPool.Scope pool;

        private static int id;

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
            var tp = Exp.Variable(type, "#Temp" + type.Name + id++);
            var temp = new VariableScope {
                Create = true,
                Name = tp.Name,
                IsTemp = true,
                InUse = true,
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


        //public override void Dispose()
        //{
        //    base.Dispose();
        //    pool.Dispose();
        //}

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

        //public Sequence<VariableScope> ClosureList
        //{
        //    get; private set;
        //}

        public VariableScope GetVariable(in StringSpan name, bool createClosure = true)
        {

            var start = this;
            while (start != null)
            {

                if (start.variableScopeList.TryGetValue(name, out var result))
                {
                    return result;
                }
                start = start.Parent;
            }

            if (!createClosure)
                throw new ArgumentOutOfRangeException($"{name} not found in current variable scope");

            return null;
        }

        //private VariableScope CreateClosure(in StringSpan name)
        //{
        //    var p = Parent;
        //    if (p == null)
        //        return null;
        //    var v = p.GetVariable(name);
        //    if (v == null)
        //        return null;
        //    ClosureList = ClosureList ?? new Sequence<VariableScope>();
        //    var v1 = new VariableScope()
        //    {
        //        Variable = Expression.Parameter(typeof(JSVariable), name.Value),
        //        Name = name.Value
        //    };
        //    int index = ClosureList.Count;
        //    if (v.Expression.NodeType == Expressions.YExpressionType.Field)
        //    {
        //        v1.Expression = JSVariable.ValueExpression(v1.Variable);
        //    } else
        //    {
        //        v1.Expression = JSVariableBuilder.Property(v1.Variable);
        //    }
        //    v1.SetInit(Expression.ArrayIndex(Closures, Expression.Constant(index)));
        //    ClosureList.Add(v);
        //    this.variableScopeList[name] = v1;
        //    return v1;
        //}


    }
}
