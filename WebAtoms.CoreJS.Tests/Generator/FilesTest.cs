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
        readonly string root;

        readonly bool saveLambda;
        public TestFolderAttribute(string root, bool saveLambda = false)
        {
            this.root = root;
            this.saveLambda = saveLambda;
        }

        private Stopwatch watch = new Stopwatch();


        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var files = GetData();
            TestResult[] result = null;
            var taskList = files.ToList();
            result = new TestResult[taskList.Count];
            watch.Start();
            AsyncPump.Run(async () =>
            {
                var tasks = taskList.Select(x => Task.Run(() => RunAsyncTest(x))).ToList();
                result = await Task.WhenAll(tasks);
            });
            watch.Stop();
            return result;
        }

        public IEnumerable<(FileInfo,string)> GetData()
        {
            IEnumerable<(FileInfo,string)> GetFiles(DirectoryInfo files)
            {
                foreach (var file in files.EnumerateFiles())
                {
                    var name = file.FullName;
                    if (!name.EndsWith(".js"))
                        continue;
                    yield return (file, files.Name + "\\" + file.Name);
                }
                foreach (var dir in files.EnumerateDirectories())
                {
                    foreach (var x in GetFiles(dir))
                        yield return x;
                }
            }
            var dir1 = new DirectoryInfo("../../../Generator/Files/" + root);
            return GetFiles(dir1);
        }

        protected virtual void Evaluate(JSTestContext context, string content, string fullName)
        {
            CoreScript.Evaluate(content, fullName, saveLambda ? AssemblyCodeCache.Instance : DictionaryCodeCache.Current);
            if (context.waitTask != null)
            {
                AsyncPump.Run(() => context.waitTask);
            }
        }
        protected async Task<TestResult> RunAsyncTest((FileInfo file, string name) testCase)
        {
            // var watch = new Stopwatch();
            // watch.Start();
            var start = watch.ElapsedMilliseconds;
            Exception lastError = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                string content;
                using (var fs = testCase.file.OpenText())
                {
                    content = await fs.ReadToEndAsync();
                }
                using (var jc = new JSTestContext())
                {
                    jc.Log += (_, s) => sb.AppendLine(s.ToDetailString());
                    jc.Error += (_, e) => lastError = e;
                    Evaluate(jc, content, testCase.file.FullName);
                }
            } catch (Exception ex)
            {
                lastError = ex;
            }
            var time = watch.ElapsedMilliseconds - start;
            return new TestResult {
                Outcome = lastError  == null ? UnitTestOutcome.Passed : UnitTestOutcome.Failed,
                DisplayName = testCase.name,
                Duration = TimeSpan.FromMilliseconds(time),
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

        protected override void Evaluate(JSTestContext context, string content, string fullName)
        {
            AsyncPump.Run(async () =>
            {
                CoreScript.Evaluate(content, fullName, DictionaryCodeCache.Current);
                if (context.waitTask != null)
                {
                    try
                    {
                        await context.waitTask;
                    }catch (TaskCanceledException) { }
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
