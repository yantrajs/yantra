using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using YantraJS;
using YantraJS.Core;
using YantraJS.Utils;
using YantraJS.REPL;

namespace Yantra
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

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
            
            var yc = new YantraContext(file.DirectoryName);
            var r = await yc.RunAsync(file.DirectoryName, "./" + file.Name);
            if (!r.IsUndefined)
                Console.WriteLine(r);
            //using var jc = new JSTestContext();
            //jc["global"] = jc;
            //var a = new Stopwatch();
            //try
            //{
            //    a.Start();
            //    CoreScript.Evaluate(script, args[1]);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}
            //a.Stop();
            //Console.WriteLine($"Total time: {a.Elapsed}");
            //var (size, total, next) = KeyString.Total;
            //Console.WriteLine($"Total: {total} Size: {size} Last Index: {next}");
        }
    }

    //public class Helper
    //{
    //    public static void Generate()
    //    {
    //        AssemblyName aName = new AssemblyName("DynamicAssemblyExample");
    //        AssemblyBuilder ab =
    //            AssemblyBuilder.DefineDynamicAssembly(
    //                aName,
    //                AssemblyBuilderAccess.RunAndCollect);

    //        var md = ab.DefineDynamicModule(aName.Name);

    //        var t = md.DefineType("JSCode", TypeAttributes.Public);

    //        var m = t.DefineMethod(
    //            "Run",
    //            MethodAttributes.Static | MethodAttributes.Public,
    //            typeof(JSValue),
    //            new Type[] { typeof(JSValue), typeof(JSValue[]) });
    //    }
    //}
}
