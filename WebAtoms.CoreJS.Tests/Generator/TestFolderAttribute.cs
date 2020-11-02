using Microsoft.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Emit;
using WebAtoms.CoreJS.Utils;
namespace WebAtoms.CoreJS.Tests.Generator
{
    public class TestFolderAttribute : TestMethodAttribute
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
                foreach (var ri in r)
                {
                    if (ri.Outcome != UnitTestOutcome.Passed)
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
            if (context.WaitTask != null)
            {
                AsyncPump.Run(() => context.WaitTask);
            }
        }

        protected virtual JSContext CreateContext(FileInfo file)
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
                using (var jc = CreateContext(file))
                {
                    jc.Log += (_, s) =>
                    {
                        lock (sb)
                        {
                            var text = s.ToDetailString();
                            sb.AppendLine(text);
                        }
                    };
                    jc.Error += (_, e) => lastError = e;
                    Evaluate(jc, content, file.FullName);
                }
            }
            catch (Exception ex)
            {
                lastError = ex;
            }
            var time = watch.ElapsedTicks - start;
            return new TestResult
            {
                Outcome = lastError == null ? UnitTestOutcome.Passed : UnitTestOutcome.Failed,
                DisplayName = file.Directory.Name + "\\" + file.Name,
                Duration = TimeSpan.FromTicks(time),
                TestFailureException = lastError,
                LogOutput = sb.ToString()
            };
        }

    }
}
