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

namespace Yantra
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            // DictionaryCodeCache.Current = new AssemblyCodeCache();

            ILCodeGenerator.GenerateLogs = true;

            if (args.Length == 0)
            {
                // no parameter....

                // start REPL
                var c = new YantraRepl();
                c.Run();
                return;
            }

            var file = new FileInfo(args[0]);
            if (!file.Exists)
                throw new FileNotFoundException(file.FullName);

            var filePath = new FileInfo(typeof(Program).Assembly.Location);
            var inbuilt = filePath.DirectoryName + "/modules";
            
            var yc = new YantraContext(file.DirectoryName);
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
    }

}
