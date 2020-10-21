using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace Yantra
{

    public delegate void ModuleDelegate(
        JSValue exports,
        JSValue require,
        JSValue module,
        string __filename,
        string __dirname
        );

    public class YantraContext: JSModuleContext
    {

        public YantraContext()
        {
            this.extensions = new string[] { ".js", ".csx" };
        }

        protected override JSFunctionDelegate Compile(string code, string filePath, List<string> args)
        {
            if (filePath.EndsWith(".csx"))
            {
                var options = ScriptOptions.Default
                        .WithFilePath(filePath)
                        .AddReferences(typeof(JSValue).Assembly, typeof(YantraContext).Assembly)
                        .WithOptimizationLevel(OptimizationLevel.Debug);

                JSModuleDelegate @delegate = null;

                AsyncPump.Run(async () => {

                    @delegate = await CSharpScript.EvaluateAsync<JSModuleDelegate>(code, options);
                    
                });
                return (in Arguments a) => {
                    var alist = a.GetArgs();
                    @delegate(alist[0], alist[1], alist[2], alist[3].ToString(), alist[4].ToString());
                    return JSUndefined.Value;
                };
            }
            return base.Compile(code, filePath, args);
        }

    }

    public class ModuleGlobals
    {
        public JSValue exports;
        public JSValue require;
        public JSValue module;
        public string __filename;
        public string __dirname;
    }
}
