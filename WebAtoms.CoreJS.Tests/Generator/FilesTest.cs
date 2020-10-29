using Microsoft.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Emit;
using WebAtoms.CoreJS.Utils;
namespace WebAtoms.CoreJS.Tests.Generator
{
    public class TestFolderAttribute: TestMethodAttribute
    {
        protected readonly string root;

        readonly bool saveLambda;

        public TestFolderAttribute(string root, bool saveLambda = false)
        {
            // DictionaryCodeCache.Current = AssemblyCodeCache.Instance;
            this.root = root;
            this.saveLambda = saveLambda;
        }

        private Stopwatch watch = new Stopwatch();


        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var files = GetData();
            var taskList = files.ToList();
            var result = new TestResult[taskList.Count];

            watch.Start();
            AsyncPump.Run(async () =>
            {
                var tasks = taskList.Select(x => Task.Run(() => RunAsyncTest(x))).ToList();
                var r = await Task.WhenAll(tasks);
                int resultIndex = 0;
                foreach(var ri in r)
                {
                    if(ri.Outcome != UnitTestOutcome.Passed)
                    {
                        result[resultIndex++] = ri;
                    }
                }
                foreach (var ri in r)
                {
                    if (ri.Outcome == UnitTestOutcome.Passed)
                    {
                        result[resultIndex++] = ri;
                    }
                }
            });
            watch.Stop();
            return result;
        }

        public virtual IEnumerable<FileInfo> GetData()
        {
            var dir1 = new DirectoryInfo("../../../Generator/Files/" + root);
            return dir1.EnumerateFiles("*.js", new EnumerationOptions { RecurseSubdirectories = true });
        }

        protected virtual void Evaluate(JSContext context, string content, string fullName)
        {
            CoreScript.Evaluate(content, fullName, saveLambda ? AssemblyCodeCache.Instance : DictionaryCodeCache.Current);
            if (context.waitTask != null)
            {
                AsyncPump.Run(() => context.waitTask);
            }
        }

        protected virtual JSContext CreateContext()
        {
            return new JSTestContext();
        }

        protected async Task<TestResult> RunAsyncTest(FileInfo file)
        {
            // var watch = new Stopwatch();
            // watch.Start();
            var start = watch.ElapsedTicks;
            Exception lastError = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                string content;
                using (var fs = file.OpenText())
                {
                    content = await fs.ReadToEndAsync();
                }
                using (var jc = CreateContext())
                {
                    jc.Log += (_, s) => sb.AppendLine(s.ToDetailString());
                    jc.Error += (_, e) => lastError = e;
                    Evaluate(jc, content, file.FullName);
                }
            } catch (Exception ex)
            {
                lastError = ex;
            }
            var time = watch.ElapsedTicks - start;
            return new TestResult {
                Outcome = lastError  == null ? UnitTestOutcome.Passed : UnitTestOutcome.Failed,
                DisplayName = file.Directory.Name + "\\" + file.Name,
                Duration = TimeSpan.FromTicks(time),
                TestFailureException = lastError,
                LogOutput = sb.ToString()
            };
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AsyncTestFolderAttribute: TestFolderAttribute
    {
        public AsyncTestFolderAttribute(string root) : base(root)
        {

        }

        protected override void Evaluate(JSContext context, string content, string fullName)
        {
            AsyncPump.Run(async () =>
            {
                // this needs to run inside AsyncPump 
                // as Promise expects SynchronizationContext to be present
                var r = CoreScript.Evaluate(content, fullName, DictionaryCodeCache.Current);
                if (context.waitTask != null)
                {
                    try
                    {
                        await context.waitTask;
                    }catch (TaskCanceledException) { }
                }
                if (r is JSPromise jp)
                {
                    await jp.Task;
                }
            });
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleFolderAttribute : TestFolderAttribute
    {
        public ModuleFolderAttribute(string root) : base(root)
        {

        }

        protected override JSContext CreateContext()
        {
            return new JSModuleContext();
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
                    await m.RunAsync(fileInfo.DirectoryName, "./" + fileInfo.Name);
                }catch (TaskCanceledException)
                {

                }
            });
        }
    }


    [TestClass]
    public class ES5
    {
        [TestFolder("es5\\Objects")]
        public void Objects()
        {
            
        }


        [TestFolder("es5\\Objects\\Date")]
        public void Date()
        {

        }

    }

    [TestClass]
    public class Statements
    {
        [TestFolder("es5\\Statements")]
        public void Run()
        {

        }

    }

    [TestClass]
    public class Syntax
    {
        [TestFolder("es5\\Syntax")]
        public void Run()
        {

        }

    }


    [TestClass]
    public class Function
    {
        [TestFolder("es5\\Function")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class ES6
    {
        [TestFolder("es6\\Syntax")]
        public void Syntax()
        {

        }

    }

    [TestClass]
    public class Modules
    {
        [ModuleFolder("es6\\Modules\\clr")]
        public void Clr()
        {

        }

    }



    [TestClass]
    public class String
    {
        [TestFolder("es5\\String")]
        public void Run()
        {
            
        }
    }

    [TestClass]
    public class Array
    {
        [TestFolder("es5\\Array")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class Number
    {
        [TestFolder("es5\\Number")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class Math
    {
        [TestFolder("es5\\Math")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class Promise
    {
        [AsyncTestFolder("es5\\Promise")]
        public void Run()
        {

        }
    }

}
