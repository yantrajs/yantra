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
        public JSGenerator(JSGeneratorDelegate @delegate, Arguments a)
        {
            this.@delegate = @delegate;
            this.a = a;
        }

        // wait by current thread...
        AutoResetEvent yield;

        // wait by generator thread...
        AutoResetEvent wait;

        public JSValue Next()
        {
            if (yield == null)
            {
                yield = new AutoResetEvent(false);
                ThreadPool.QueueUserWorkItem(RunGenerator, this);
            }
            wait.Set();
            yield.WaitOne();
            return value;
        }

        internal JSValue Yield(JSValue value)
        {
            yield.Set();
            this.value = value;
            wait.WaitOne();
            return this.value;
        }

        private static void RunGenerator(object state)
        {
            JSGenerator generator = state as JSGenerator;
            generator.@delegate(generator, generator.a);
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
