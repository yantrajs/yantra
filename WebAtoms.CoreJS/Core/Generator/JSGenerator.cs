using System;
using System.Threading;

namespace WebAtoms.CoreJS.Core.Generator
{
    public class JSGenerator : JSObject
    {
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

        // wait by current thread...
        AutoResetEvent yield;

        // wait by generator thread...
        AutoResetEvent wait;
        Thread thread;

        public JSValue Next(JSValue replaceOld = null)
        {
            if (replaceOld != null)
            {
                this.value = replaceOld;
            }
            if (this.done)
            {
                return (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, JSUndefined.Value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
            }
            if (yield == null)
            {
                yield = new AutoResetEvent(false);
                wait = new AutoResetEvent(false);
                // using ThreadPool could be dangerous as it might run on somebody
                // else's thread creating conflicts...
                // ThreadPool.QueueUserWorkItem(RunGenerator, this);
                this.thread = new Thread(RunGenerator);
                thread.Start(this);

                while (!thread.IsAlive) ;
            }

            wait.Set();
            yield.WaitOne();

            if (this.done)
            {
                return (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, JSUndefined.Value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
            }

            return (JSObject.NewWithProperties())
                .AddProperty(KeyStrings.value, value)
                .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
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

        private static void RunGenerator(object state)
        {
            JSGenerator generator = state as JSGenerator;
            JSContext.Current = generator.context;
            // generator.yield.Set();
            generator.wait.WaitOne();
            generator.@delegate(generator, generator.a);
            generator.done = true;
            generator.yield.Set();
        }

        [Prototype("next")]
        public static JSValue Next(in Arguments a)
        {
            if(!(a.This is JSGenerator generator))
            {
                throw JSContext.Current.NewTypeError($"receiver for Generator.prototype.next should be generator");
            }
            return generator.Next(a.Length == 0 ? null : a.Get1());
        }
    }
}
