using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
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
                var script = CSharpScript.Create<ModuleDelegate>(code,
                    ScriptOptions.Default
                        .WithFilePath(filePath)
                        .AddReferences(typeof(JSValue).Assembly, typeof(YantraContext).Assembly)
                        .WithOptimizationLevel(OptimizationLevel.Release)).CreateDelegate();

                return (in Arguments a) => {
                    var alist = a.GetArgs();
                    var clrList = new object[] { alist[0], alist[1], alist[2], alist[3].ToString(), alist[4].ToString() };
                    script.Method.Invoke(null, clrList);
                    return JSUndefined.Value;
                };
            }
            return base.Compile(code, filePath, args);
        }

    }
}
