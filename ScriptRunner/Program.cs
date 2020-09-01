using System;
using System.Threading.Tasks;
using WebAtoms.CoreJS;
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
                CoreScript.Evaluate(script, args[1]);
            }
        }
    }
}
