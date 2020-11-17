using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace YantraJS.Core.Generator
{
    public delegate void JSGeneratorDelegate(in JSWeakGenerator generator, in Arguments a);
    public class JSGeneratorFunction: JSFunction
    {
        readonly JSGeneratorDelegate @delegate;

        public JSGeneratorFunction(JSGeneratorDelegate @delegate, in StringSpan name, in StringSpan code): 
            base(null, name, code)
        {
            this.@delegate = @delegate;
            this.f = InvokeFunction;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            return new JSGenerator(@delegate, a);
        }

    }
}
