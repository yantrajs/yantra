using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace WebAtoms.CoreJS.Core.Generator
{
    public struct JSWeakGenerator
    {
        private WeakReference<JSGenerator> generator;

        public JSWeakGenerator(JSGenerator g)
        {
            this.generator = new WeakReference<JSGenerator>(g);
        }

        public JSValue Yield(JSValue value)
        {
            if (!generator.TryGetTarget(out var g))
                throw new ObjectDisposedException("Generator has been disposed");
            return g.Yield(value);
        }
    }

    public class JSGenerator : JSObject
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

        readonly JSGeneratorDelegate @delegate;
        readonly Arguments a;
        JSValue value;
        bool done;
        JSContext context;
        public JSGenerator(JSGeneratorDelegate @delegate, Arguments a)
        {
            this.prototypeChain = JSContext.Current.GeneratorPrototype;
            this.@delegate = @delegate;
            this.context = JSContext.Current;
            this.a = a;
            done = false;
        }

        ~JSGenerator()
        {
            thread?.Abort();
            yield?.Dispose();
            wait?.Dispose();
        }


        Thread thread;

        public JSValue Return(JSValue value)
        {
            this.done = true;
            this.value = JSUndefined.Value;
            thread?.Abort();
            yield?.Dispose();
            wait?.Dispose();
            return (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
        }

        public JSValue Throw(JSValue value)
        {
            thread?.Abort();
            return ValueObject;
        }

        public JSValue ValueObject => (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, this.value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
        private Exception lastError;

        public JSValue Next(JSValue replaceOld = null)
        {
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
                this.thread = new Thread(RunGenerator);
                thread.Start(this);
            } else
            {
                wait.Set();
            }

            yield.WaitOne();

            if (this.lastError != null)
                throw lastError;

            if (this.done)
            {
                this.value = JSUndefined.Value;
                yield?.Dispose();
                wait?.Dispose();
                return ValueObject;
            }

            return ValueObject;
        }

        public JSValue Yield(JSValue value)
        {
            yield.Set();
            this.value = value;
            wait.WaitOne();
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

        internal override IEnumerator<JSValue> GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        private struct ElementEnumerator: IEnumerator<JSValue>
        {
            readonly JSGenerator generator;
            public ElementEnumerator(JSGenerator generator)
            {
                this.generator = generator;
            }

            public JSValue Current => generator.value;

            object IEnumerator.Current => generator.value;

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                return generator.Next().BooleanValue;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        private static void RunGenerator(object state)
        {
            JSGenerator generator = state as JSGenerator;
            try
            {
                JSContext.Current = generator.context;
                // generator.yield.Set();
                // generator.wait.WaitOne();
                generator.@delegate(generator, generator.a);
                generator.done = true;
                generator.yield.Set();
            }catch (Exception ex)
            {
                generator.lastError = ex;
                generator.yield.Set();
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

    }
}
