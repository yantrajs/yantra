using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core.Generator
{
    public delegate void JSGeneratorDelegate(JSGenerator generator, in Arguments a);

    public class JSGenerator : JSObject
    {
        readonly JSGeneratorDelegate @delegate;
        readonly Arguments a;
        JSValue value;
        bool done;
        public JSGenerator(JSGeneratorDelegate @delegate, Arguments a)
        {
            this.@delegate = @delegate;
            this.a = a;
            done = false;
        }

        // wait by current thread...
        AutoResetEvent yield;

        // wait by generator thread...
        AutoResetEvent wait;

        public JSValue Next()
        {
            if (this.done)
            {
                return (new JSObject())
                    .AddProperty(KeyStrings.value, value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
            }
            if (yield == null)
            {
                yield = new AutoResetEvent(false);
                wait = new AutoResetEvent(false);
                ThreadPool.QueueUserWorkItem(RunGenerator, this);
                yield.WaitOne();
            }
            wait.Set();
            yield.WaitOne();
            return (new JSObject())
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
                throw JSContext.Current.NewTypeError($"value is not generator");
            while(!generator.done)
            {
                this.Yield(generator.Next());
            }
            return this.value;
        }

        private static void RunGenerator(object state)
        {
            JSGenerator generator = state as JSGenerator;
            generator.yield.Set();
            generator.wait.WaitOne();
            generator.@delegate(generator, generator.a);
            generator.done = true;
            generator.yield.Set();
        }
    }

    public class JSGeneratorFunction: JSFunction
    {
        readonly JSGeneratorDelegate @delegate;

        public JSGeneratorFunction(JSGeneratorDelegate @delegate): base(JSFunction.empty)
        {
            this.@delegate = @delegate;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            return new JSGenerator(@delegate, a);
        }

    }
}
