using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using YantraJS.Core.LightWeight;

namespace YantraJS.Core.Generator
{
    public class JSGenerator : JSObject, IDisposable
    {

        /**
         * Using ManualResetEventSlim is of no use as it blocks endlessly when `Set` is applied
         * before `Wait` and which causes singal loss leading to deadlock.
         */


        // wait by current thread...
        // AutoResetEvent yield;
        private AutoResetEvent yield;

        // wait by generator thread...
        private AutoResetEvent wait;
        private readonly JSVariable[] closures;
        readonly JSGeneratorDelegate @delegate;
        readonly Arguments a;
        internal JSValue value;
        internal bool done;
        private Exception lastError;
        // private LightWeightStack<CallStackItem> threadTop;
        private CallStackItem threadTop;
        readonly IElementEnumerator en;
        private readonly string name;

        private JSContext context;

        public JSGenerator(JSVariable[] closures, JSGeneratorDelegate @delegate, Arguments a)
        {
            context = JSContext.Current;
            this.prototypeChain = JSContext.Current.GeneratorPrototype;
            this.closures = closures;
            this.@delegate = @delegate;
            this.a = a;
            done = false;
            name = "generator";
        }

        public JSGenerator(IElementEnumerator en, string name) {
            this.prototypeChain = JSContext.Current.GeneratorPrototype;
            this.en = en;
            this.name = name;
        }

        public override string ToString()
        {
            return $"[object {name}]";
        }

        [Prototype("toString")]
        public static JSValue ToString(in Arguments a)
        {
            var a1 = a.This as JSGenerator;
            return new JSString(a1.ToString());
        }

        ~JSGenerator()
        {
            OnDispose(false);
        }


        // Thread thread;

        public JSValue Return(JSValue value)
        {
            this.done = true;
            this.value = JSUndefined.Value;
            yield?.Set();
            wait?.Set();
            return (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
        }

        public JSValue Throw(JSValue value)
        {
            yield?.Dispose();
            wait?.Dispose();
            yield = null;
            wait = null;
            return ValueObject;
        }

        public JSValue ValueObject => (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, this.value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);

        public JSValue Next(JSValue replaceOld = null)
        {
            if (en != null) {
                if (en.MoveNext(out var item)) {
                    this.value = item;
                    return ValueObject;
                }
                this.done = true;
                this.value = JSUndefined.Value;
                return ValueObject;
            }

            // var current = JSContext.Current.CloneStack();
            var current = context.Top;
            if (replaceOld != null)
            {
                this.value = replaceOld;
            }
            if (this.done)
            {
                this.value = JSUndefined.Value;
                return ValueObject;
            }
            if (wait == null)
            {
                wait = new AutoResetEvent(false);
                yield = new AutoResetEvent(false);
                // this.thread = new Thread(RunGenerator);
                // thread.Start(new JSWeakGenerator(this));
                threadTop = JSContext.Current.Top;
                JSThreadPool.Queue(RunGenerator, new JSWeakGenerator(this));
            } else
            {
                context.Top = threadTop;
                wait.Set();
            }

            yield.WaitOne(Timeout.Infinite);

            threadTop = context.Top;
            context.Top = current;

            if (this.lastError != null)
            {
                this.OnDispose();
                throw lastError;
            }

            if (this.done)
            {
                this.value = JSUndefined.Value;
                this.OnDispose();
                return ValueObject;
            }

            return ValueObject;
        }

        public JSValue Yield(JSValue value)
        {
            try
            {
                this.value = value;
                yield.Set();
                wait.WaitOne(Timeout.Infinite);
            } catch (ObjectDisposedException)
            {
                throw new SafeExitException();
            }
            return this.value;
        }

        public JSValue Delegate(JSValue value)
        {
            if (!(value is JSGenerator generator))
            {
                do {
                    var a = value.InvokeMethod(KeyStrings.next, new Arguments(this));
                    if(a[KeyStrings.done].BooleanValue)
                    {
                        break;
                    }
                    this.Yield(a[KeyStrings.value]);
                } while (true);
                return this.value;
            }
            while (!generator.done)
            {
                this.Yield(generator.Next());
            }
            return this.value;
        }

        internal override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        private struct ElementEnumerator: IElementEnumerator
        {
            readonly JSGenerator generator;
            int index;
            public ElementEnumerator(JSGenerator generator)
            {
                this.generator = generator;
                index = -1;
            }

            public bool MoveNext(out JSValue value)
            {
                generator.Next();
                if (!generator.done)
                {
                    this.index++;
                    value = this.generator.value;
                    return true;
                }
                value = JSUndefined.Value;
                return false;

            }


            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                generator.Next();
                if (!generator.done)
                {
                    this.index++;
                    index = (uint)this.index;
                    hasValue = true;
                    value = this.generator.value;
                    return true;
                }
                index = 0;
                value = JSUndefined.Value;
                hasValue = false;
                return false;

            }
        }

        private static void RunGenerator(object state)
        {
            try
            {
                var weakGenerator = (JSWeakGenerator)state;
                try
                {
                    JSGeneratorDelegate @delegate;
                    Arguments a;
                    JSVariable[] closures;
                    if (weakGenerator.generator.TryGetTarget(out var generator))
                    {
                        @delegate = generator.@delegate;
                        closures = generator.closures;
                        a = generator.a;
                    }
                    else
                    {
                        return;
                    }
                    generator = null;
                    @delegate(closures, in weakGenerator, in a);
                    if (weakGenerator.generator.TryGetTarget(out generator))
                    {
                        generator.done = true;
                        try
                        {
                            generator.yield.Set();
                        }
                        catch (ObjectDisposedException)
                        {
                            throw new SafeExitException();
                        }
                    }
                }
                catch (SafeExitException)
                {
                    // do nothing..
                }
                catch (Exception ex)
                {
                    if (weakGenerator.generator.TryGetTarget(out var g))
                    {
                        g.lastError = ex;
                        g.yield.Set();

                    }
                }
            } 
            catch (ObjectDisposedException)
            {
                // do nothing...
            }
        }

        [Prototype("next", Length = 1)]
        public static JSValue Next(in Arguments a)
        {
            if(!(a.This is JSGenerator generator))
            {
                throw JSContext.Current.NewTypeError($"receiver for Generator.prototype.next should be generator");
            }
            return generator.Next(a.Length == 0 ? null : a.Get1());
        }

        [Prototype("return", Length = 1)]
        public static JSValue Return(in Arguments a)
        {
            if (!(a.This is JSGenerator generator))
            {
                throw JSContext.Current.NewTypeError($"receiver for Generator.prototype.next should be generator");
            }
            return generator.Return(a.Get1());
        }

        [Prototype("throw", Length = 1)]
        public static JSValue Throw(in Arguments a)
        {
            if (!(a.This is JSGenerator generator))
            {
                throw JSContext.Current.NewTypeError($"receiver for Generator.prototype.next should be generator");
            }
            return generator.Return(a.Get1());
        }

        private void OnDispose(bool supress = true)
        {
            threadTop?.Pop(context);
            threadTop = null;
            yield?.Dispose();
            wait?.Dispose();
            yield = null;
            wait = null;
            if (supress)
            {
                GC.SuppressFinalize(this);
            }
        }

        void IDisposable.Dispose()
        {
            OnDispose();
        }
    }
}
