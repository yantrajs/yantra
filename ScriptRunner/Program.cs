using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebAtoms.CoreJS;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Utils;

namespace ScriptRunner
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var script = await System.IO.File.ReadAllTextAsync(args[0]);
            using (var jc = new JSTestContext())
            {
                var a = new Stopwatch();
                try
                {
                    a.Start();
                    CoreScript.Evaluate(script, args[1]);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                a.Stop();
                Console.WriteLine($"Total time: {a.Elapsed}");
                var t = KeyString.Total;
                Console.WriteLine($"Total: {t.total} Size: {t.size}");
            }
        }
    }
}
