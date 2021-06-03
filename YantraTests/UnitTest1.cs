using Microsoft.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YantraJS.Core;
using Yantra;
using YantraJS.Tests.Generator;
using YantraJS;

namespace YantraTests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleFolderAttribute : TestFolderAttribute
    {
        public ModuleFolderAttribute(string root) : base(root)
        {
            this.Parallel = false;
        }

        private static DirectoryInfo rootModules;

        public override IEnumerable<System.IO.FileInfo> GetData()
        {
            if(rootModules == null) {
                rootModules = new DirectoryInfo("../../../../modules/inbuilt");
            }
            var dir1 = new DirectoryInfo("../../../Modules/" + root);
            foreach (var dir in dir1.EnumerateDirectories()) {
                var indexFile = new FileInfo(dir.FullName + "/index.js");
                if (indexFile.Exists)
                {
                    yield return indexFile;
                }
            }
        }

        protected override JSContext CreateContext(FileInfo file)
        {
            return new YantraContext(file.DirectoryName);
        }

        protected override void Evaluate(JSContext context, string content, string fullName)
        {
            // do not run if there is no package.json in same folder...

            AsyncPump.Run(async () =>
            {
                // this needs to run inside AsyncPump 
                // as Promise expects SynchronizationContext to be present
                // CoreScript.Evaluate(content, fullName, DictionaryCodeCache.Current);
                var m = context as JSModuleContext;
                var fileInfo = new System.IO.FileInfo(fullName);
                try
                {
                    await m.RunAsync(fileInfo.DirectoryName, "./" + fileInfo.Name, new string[] {
                        rootModules.FullName,
                        rootModules.FullName + "/bin"
                    });
                }
                catch (TaskCanceledException)
                {

                }
            });
        }
    }

    [TestClass]
    public class Modules
    {
        [ModuleFolder("in-built")]
        public void TestMethod1()
        {
        }
    }
}
