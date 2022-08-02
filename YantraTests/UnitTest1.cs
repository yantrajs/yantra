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
using System.Threading;

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

        protected override JSContext CreateContext(FileInfo file, SynchronizationContext ctx)
        {
            return new YantraContext(file.DirectoryName, ctx);
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
                await m.RunAsync(fileInfo.DirectoryName, "./" + fileInfo.Name, new string[] {
                    rootModules.FullName,
                    rootModules.FullName + "/bin"
                });
            }
            catch (TaskCanceledException)
            {

            }
        }
    }

    [TestClass]
    public class Modules
    {

        [ModuleFolder("in-built")]
        public void TestMethod1()
        {
        }

        //private static DirectoryInfo rootModules;
        //public static IEnumerable<object[]> Files {
        //    get
        //    {
        //        if (rootModules == null)
        //        {
        //            rootModules = new DirectoryInfo("../../../../modules/inbuilt");
        //        }
        //        var dir1 = new DirectoryInfo("../../../Modules/in-built");
        //        foreach (var dir in dir1.EnumerateDirectories())
        //        {
        //            var indexFile = new FileInfo(dir.FullName + "/index.js");
        //            if (indexFile.Exists)
        //            {
        //                yield return new object[] { indexFile };
        //            }
        //        }

        //    }
        //}

        //[TestMethod]
        //[DynamicData(nameof(Files))]
        //public async Task TestMethod1(FileInfo file)
        //{
        //    string content;
        //    using (var fs = file.OpenText())
        //    {
        //        content = await fs.ReadToEndAsync();
        //    }
        //    using (var jc = new YantraContext(file.DirectoryName))
        //    {
        //        jc.Log += (_, s) =>
        //        {
        //            var text = s.ToDetailString();
        //            System.Diagnostics.Trace.WriteLine(text);
        //        };
        //        // jc.Error += (_, e) => lastError = e;
        //        await jc.RunAsync(file.DirectoryName, "./" + file.Name, new string[] {
        //            rootModules.FullName,
        //            rootModules.FullName + "/bin"
        //        });
        //    }
        //}
    }
}
