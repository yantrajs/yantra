using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace WebAtoms.CoreJS.Core.Generator
{
    public struct JSWeakGenerator
    {
        internal readonly WeakReference<JSGenerator> generator;

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

        public JSValue Delegate(JSValue value)
        {
            if (!generator.TryGetTarget(out var g))
                throw new ObjectDisposedException("Generator has been disposed");
            return g.Delegate(value);
        }

    }

    internal class SafeExitException: Exception
    {
        public SafeExitException()
        {

        }
    }

    public struct JSGeneratorEnumerator: IEnumerator<(uint Key, JSProperty Value)>
    {
        JSGenerator g;
        uint index;
        public JSGeneratorEnumerator(JSGenerator g)
        {
            this.g = g;
            index = 0;
        }

        public (uint Key, JSProperty Value) Current => (index-1, JSProperty.Property(g.value) );

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            this.g.Next();
            return !g.done;
        }

        public void Reset()
        {
            
        }
    }

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

        readonly JSGeneratorDelegate @delegate;
        readonly Arguments a;
        internal JSValue value;
        internal bool done;
        public JSGenerator(JSGeneratorDelegate @delegate, Arguments a)
        {
            this.prototypeChain = JSContext.Current.GeneratorPrototype;
            this.@delegate = @delegate;
            this.a = a;
            done = false;
        }

        ~JSGenerator()
        {
            OnDispose(false);
        }


        Thread thread;

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
                thread.Start(new JSWeakGenerator(this));
            } else
            {
                wait.Set();
            }

            yield.WaitOne();

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
                yield.Set();
                this.value = value;
                wait.WaitOne();
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

            public uint Index => (uint)index;
            public JSValue Current => generator.value;


            public bool MoveNext()
            {
                if (generator.Next().BooleanValue)
                {
                    index++;
                    return true;
                }
                return false;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if (generator.Next().BooleanValue)
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

            public bool TryGetCurrent(out JSValue value)
            {
                value = this.generator.value;
                return true;
            }

            public bool TryGetCurrent(out JSValue value, out uint index)
            {
                value = this.generator.value;
                index = (uint)this.index;
                return true;
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
                    if (weakGenerator.generator.TryGetTarget(out var generator))
                    {
                        @delegate = generator.@delegate;
                        a = generator.a;
                    }
                    else
                    {
                        return;
                    }
                    generator = null;
                    @delegate.Invoke(weakGenerator, a);
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
