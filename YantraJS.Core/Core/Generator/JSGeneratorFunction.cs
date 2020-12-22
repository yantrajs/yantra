using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YantraJS.Core.CodeGen;
using YantraJS.Core.LinqExpressions.Generators;

namespace YantraJS.Core.Generator
{
    public delegate void JSGeneratorDelegate(ScriptInfo script, JSVariable[] closures, ClrGenerator g, in Arguments a);
    public class JSGeneratorFunction: JSFunction
    {
        private readonly ScriptInfo script;
        private readonly JSVariable[] closures;
        readonly JSGeneratorDelegate @delegate;

        public JSGeneratorFunction(
            ScriptInfo script,
            JSVariable[] closures, 
            JSGeneratorDelegate @delegate, 
            in StringSpan name, in StringSpan code): 
            base(null, name, code)
        {
            this.script = script;
            this.closures = closures;
            this.@delegate = @delegate;
            this.f = InvokeFunction;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            var g = new ClrGenerator();
            @delegate(script, closures, g, a);
            return new JSGenerator(g);
        }

    }
}
