using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebAtoms.CoreJS;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Utils;

namespace Yantra
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var script = System.IO.File.ReadAllText(args[0]);
            using var jc = new JSTestContext();
            jc["global"] = jc;
            var a = new Stopwatch();
            try
            {
                a.Start();
                CoreScript.Evaluate(script, args[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            a.Stop();
            Console.WriteLine($"Total time: {a.Elapsed}");
            var (size, total, next) = KeyString.Total;
            Console.WriteLine($"Total: {total} Size: {size} Last Index: {next}");
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
