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
            Parallel.ForEach(taskList, (x, c, i) =>
            {
                result[i] = RunTest(x);
            });
            //AsyncPump.Run(async () =>
            //{
            //    var tasks = taskList.Select(x => Task.Run(() => RunTest(x))).ToList();
            //    result = await Task.WhenAll(tasks);
            //});
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

        protected TestResult RunTest((FileInfo file, string name) testCase)
        {
            // var watch = new Stopwatch();
            // watch.Start();
            var start = watch.ElapsedMilliseconds;
            Exception lastError = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                AsyncPump.Run( async () =>
                {
                    string content;
                    using (var fs = testCase.file.OpenText())
                    {
                        content = await fs.ReadToEndAsync();
                    }
                    using var jc = new JSTestContext();
                    jc.Log += (_, s) => sb.AppendLine(s.ToDetailString());
                    jc.Error += (_, e) => lastError = e;
                    CoreScript.Evaluate(content, testCase.file.FullName);
                });
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


    [TestClass]
    public class ES5
    {
        [TestFolder("es5\\Objects")]
        public void Objects()
        {
            
        }

        [TestFolder("es5\\Statements")]
        public void Statements()
        {

        }

        [TestFolder("es5\\Syntax")]
        public void Syntax()
        {

        }

        [TestFolder("es5\\Function")]
        public void Function()
        {

        }

        [TestFolder("es5\\Objects\\Date")]
        public void Date()
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
        [TestFolder("es5\\Promise")]
        public void Run()
        {

        }
    }

}
