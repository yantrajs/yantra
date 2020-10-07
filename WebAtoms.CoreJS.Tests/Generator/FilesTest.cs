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
using WebAtoms.CoreJS.Utils;
namespace WebAtoms.CoreJS.Tests.Generator
{
    public class TestFolderAttribute: TestMethodAttribute
    {
        readonly string root;
        public TestFolderAttribute(string root)
        {
            this.root = root;
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
            static IEnumerable<(FileInfo,string)> GetFiles(DirectoryInfo files)
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
            var dir = new DirectoryInfo("../../../Generator/Files/" + root);
            return GetFiles(dir);
        }

        protected virtual void Evaluate(string content, string fullName)
        {
            CoreScript.Evaluate(content, fullName);
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
                using var jc = new JSTestContext();
                jc.Log += (_, s) => sb.AppendLine(s.ToDetailString());
                jc.Error += (_, e) => lastError = e;
                Evaluate(content, testCase.file.FullName);
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

        protected override void Evaluate(string content, string fullName)
        {
            AsyncPump.Run(() =>
            {
                base.Evaluate(content, fullName);
                return Task.FromResult(0);
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


        [AsyncTestFolder("es5\\Objects\\Date")]
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
