using Microsoft.Threading;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Core;
namespace YantraJS.Tests.Generator
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleFolderAttribute : TestFolderAttribute
    {
        public ModuleFolderAttribute(string root) : base(root)
        {

        }

        protected override JSContext CreateContext(FileInfo file, SynchronizationContext ctx)
        {
            return new JSModuleContext(ctx);
        }

        protected override async Task EvaluateAsync(JSContext context, string content, string fullName)
        {
            // do not run if there is no package.json in same folder...

            // this needs to run inside AsyncPump 
            // as Promise expects SynchronizationContext to be present
            // CoreScript.Evaluate(content, fullName, DictionaryCodeCache.Current);
            var m = context as JSModuleContext;
            var fileInfo = new System.IO.FileInfo(fullName);
            try
            {
                await m.RunAsync(fileInfo.DirectoryName, "./" + fileInfo.Name);
            }
            catch (TaskCanceledException)
            {

            }
        }
    }
}
