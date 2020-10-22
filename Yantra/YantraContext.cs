using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

                JSModuleDelegate @delegate = null;
                AsyncPump.Run(async () =>
                {
                    if (System.IO.File.Exists(filePath + ".dll"))
                    {
                        var returnType = typeof(Task<JSModuleDelegate>);
                        var a = Assembly.LoadFile(filePath + ".dll");
                        var p = a.GetTypes()
                            .SelectMany(x => x.GetMethods())
                            .FirstOrDefault(x => x.IsStatic && x.ReturnType == returnType);
                        var sa = new object[2];
                        var r = p.Invoke(null, new object[] { sa });
                        @delegate =  await (Task<JSModuleDelegate>)r;
                    } else { 
                        var options = ScriptOptions.Default
                                .WithFilePath(filePath)
                                .AddReferences(typeof(JSValue).Assembly, typeof(YantraContext).Assembly)
                                .WithOptimizationLevel(OptimizationLevel.Debug);



                            var r = await CSharpScript.RunAsync<JSModuleDelegate>(code, options);
                            @delegate = r.ReturnValue;
                            r.Script.GetCompilation().Emit(filePath + ".dll");

                    }
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
