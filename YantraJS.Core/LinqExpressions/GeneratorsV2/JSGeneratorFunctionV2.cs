using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.CodeGen;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public delegate GeneratorState JSGeneratorDelegateV2(ClrGeneratorV2 generator, in Arguments a, int nextJump, JSValue nextValue, Exception ex);

    public class JSGeneratorFunctionV2: JSFunction
    {
        readonly JSGeneratorDelegateV2 @delegate;

        public JSGeneratorFunctionV2(
            JSGeneratorDelegateV2 @delegate,
            in StringSpan name, in StringSpan code) :
            base(null, name, code)
        {
            this.@delegate = @delegate;
            this.f = InvokeFunction;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            // var c = new CallStackItem(script.FileName, this.name, 0, 0);
            return new Generator.JSGenerator( new ClrGeneratorV2(this, @delegate, a));
        }
    }

    public class GeneratorState
    {
        public readonly bool HasValue;
        public readonly JSValue Value;
        public readonly bool IsValueDelegate;
        public readonly int NextJump;

        public GeneratorState(JSValue value, int nextJump, bool isValueDelegate)
        {
            this.HasValue = value != null;
            this.Value = value;
            this.NextJump = nextJump;
            IsValueDelegate = isValueDelegate;  
        }
    }

    public class TryBlock
    {
        public int Catch;
        public int Finally;
        public int End;
        public bool CatchBegan;
        public bool FinallyBegan;
        public TryBlock Parent;
    }

    public class ClrGeneratorV2
    {
        public CallStackItem StackItem;

        private Exception lastError = null;
        private Exception injectedException = null;


        internal void InjectException(Exception ex)
        {
            this.injectedException = ex;
        }

        public Box[] Variables;
        private JSGeneratorFunctionV2 generator;
        private JSGeneratorDelegateV2 @delegate;
        private readonly Arguments arguments;

        private IElementEnumerator delegatedEnumerator;

        public JSValue LastValue;

        public JSContext Context;

        public bool IsFinished;
        public int NextJump;

        // this is null...
        public TryBlock Root;

        public ClrGeneratorV2(
            // CallStackItem stackItem, 
            JSGeneratorFunctionV2 generator,
            JSGeneratorDelegateV2 @delegate,
            Arguments arguments)
        {
            // this.StackItem = stackItem;
            this.generator = generator;
            this.@delegate = @delegate;
            this.arguments = arguments;
            this.Context = JSContext.Current;
        }

        public void InitVariables(int i)
        {
            Variables ??= new Box[i];
        }

        public Box<T> GetVariable<T>(int i)
        {
            var b = Variables[i];
            if (b == null)
            {
                b = new Box<T>();
                Variables[i] = b;
            }
            return b as Box<T>;
        }

        internal void Next(JSValue next, out JSValue value, out bool done)
        {
            if (this.delegatedEnumerator != null)
            {
                if(this.delegatedEnumerator.MoveNext(out value))
                {
                    done = false;
                    return;
                }
                this.delegatedEnumerator = null;
            }

            LastValue = next ?? LastValue ?? JSUndefined.Value;

            var v = GetNext(this.NextJump, LastValue);
            NextJump = v.NextJump;
            if (v.HasValue)
            {
                if (v.IsValueDelegate)
                {
                    this.delegatedEnumerator = v.Value.GetElementEnumerator();
                    Next(next, out value, out done);
                    return;
                }
                value = v.Value;

                if(v.NextJump == 0 || v.NextJump == -1) {

                    // need to execute finally.. if it is there...
                    if (this.Root != null && this.Root.Finally > 0)
                    {
                        v = GetNext(this.Root.Finally, value);
                        if (v.IsValueDelegate)
                        {
                            this.delegatedEnumerator = v.Value.GetElementEnumerator();
                            Next(next, out value, out done);
                            return;
                        }
                        //if (v.HasValue)
                        //{
                        //    value = v.Value;
                        //}
                    }

                    done = true;
                    return;
                }
                done = false;
                return;
            }
            done = true;
            value = default;
        }

        private GeneratorState GetNext(int nextJump, JSValue lastValue, Exception nextExp = null)
        {
            try {

                var ie = injectedException;
                if(ie != null)
                {
                    this.injectedException = null;
                    throw ie;
                }

                var r = @delegate(this, in this.arguments, nextJump, lastValue, nextExp);
                // this is case of try end and catch end...
                if (!r.HasValue && r.NextJump > 0)
                {
                    var s = GetNext(r.NextJump, lastValue);
                    return s;
                }
                return r;
            } catch (Exception ex)
            {
                var root = this.Root;
                if(root != null)
                {

                    if(root.CatchBegan || root.FinallyBegan)
                    {
                        throw;
                    }

                    // this.Root = root.Parent;
                    if (root.Catch > 0)
                    {
                        return GetNext(root.Catch, lastValue, ex);
                    }

                    if (root.Finally > 0)
                    {
                        lastError = ex;
                        var v = GetNext(root.Finally, lastValue);
                        if (v.HasValue)
                            return v;
                    }
                    throw;
                }
                throw;
            }
        }

        public void PushTry(int @catch, int @finally, int end){
            if (@catch == 0 && @finally == 0)
                throw new ArgumentException("Both catch and finally cannot be empty");
            Root = new TryBlock {
                Catch = @catch,
                Finally = @finally,
                End = end,
                Parent = Root
            };
        }


        public void Pop()
        {
            if (Root == null)
                throw new InvalidOperationException();
            Root = Root.Parent;
        }

        public void Throw(int end)
        {
            if(Root?.End == end && lastError != null)
            {
                Pop();
                throw lastError;
            }
        }

        public void BeginFinally()
        {
            this.Root.CatchBegan = false;
            this.Root.FinallyBegan = true;
        }

        public void BeginCatch()
        {
            this.Root.CatchBegan = true;
        }
    }
}
