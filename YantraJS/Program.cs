#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using YantraJS;
using YantraJS.Core;
using YantraJS.Utils;
using YantraJS.REPL;
using YantraJS.Emit;
using YantraJS.Generator;
using System.Collections.Generic;
using YantraJS.Core.Debugger;
using System.Threading;

namespace Yantra
{
    public class Program
    {
        private static Dictionary<string, string> parameters = new Dictionary<string, string>();

        public static async Task Main(string[] args)
        {

            args = GetParameters(args);

            // DictionaryCodeCache.Current = new AssemblyCodeCache();

            ILCodeGenerator.GenerateLogs = true;

            V8InspectorProtocol? inspector = null;

            using var ct = new CancellationTokenSource();

            if(parameters.TryGetValue("inspect", out var inspect))
            {
                if(inspect.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    inspector = new V8InspectorProtocolServer();
                    if (inspector != null)
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        Task.Run(() => inspector.RunAsync(ct.Token));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }

                }
            }

            if(!parameters.TryGetValue("--script", out var script))
            {
                var c = new YantraRepl();
                inspector?.AddContext(c);
                c.Run();
                ct.Cancel();
                return;
            }

            var file = new FileInfo(script);
            if (!file.Exists)
                throw new FileNotFoundException(file.FullName);

            var filePath = new FileInfo(typeof(Program).Assembly.Location);
            var inbuilt = filePath.DirectoryName + "/modules";
            
            var yc = new YantraContext(file.DirectoryName);
            inspector?.AddContext(yc);
            var r = await yc.RunAsync(
                file.DirectoryName, "./" + file.Name, 
                new string[] { 
                    file.DirectoryName,
                    file.DirectoryName + "/node_modules",
                    inbuilt
                });
            if (!r.IsUndefined)
                Console.WriteLine(r);
            Console.WriteLine(DateTime.Now);
        }

        private static string[] GetParameters(string[] args)
        {
            var empty = new string[] { };
            var en = args.GetEnumerator();
            while (en.MoveNext())
            {
                var p = en.Current!.ToString();
                if (p.StartsWith("--"))
                {
                    var tokens = p.Split(new char[] { ':' }, 2);
                    parameters[tokens[0].Substring(2)] = tokens[1];
                    continue;
                } else
                {
                    parameters["--script"] = p;
                }
            }
            return empty;
        }
    }

}
