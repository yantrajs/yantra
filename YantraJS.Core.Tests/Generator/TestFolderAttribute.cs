using Microsoft.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Emit;
using YantraJS.Generator;
using YantraJS.Utils;
namespace YantraJS.Tests.Generator
{
    public class TestFolderAttribute : TestMethodAttribute
    {
        protected readonly string root;

        readonly bool saveLambda;

        public TestFolderAttribute(string root, bool saveLambda = false)
        {
            // DictionaryCodeCache.Current = AssemblyCodeCache.Instance;
            ILCodeGenerator.GenerateLogs = true;
            this.root = root;
            this.saveLambda = saveLambda;
        }

        private Stopwatch watch = new Stopwatch();

        public bool Parallel = false;

        private TestResult[] ExecuteSync(ITestMethod  testMethod)
        {
            var files = GetData();
            var taskList = files.ToList();
            var result = new TestResult[taskList.Count];


            watch.Start();
            AsyncPump.Run(async () =>
            {
                int i = 0;
                var r = new TestResult[taskList.Count];
                foreach (var task in taskList)
                {
                    System.Diagnostics.Debug.WriteLine("Testing " + task.FullName);
                    Console.WriteLine("Testing " + task.FullName);
                    var ri = await RunAsyncTest(task);
                    if(ri.Outcome == UnitTestOutcome.Passed)
                    {
                        System.Diagnostics.Debug.WriteLine("Test done " + task.FullName);
                        Console.WriteLine("Test Done " + task.FullName);
                    }
                    r[i++] = ri;
                }
                int resultIndex = 0;
                // display errors first...
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

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            if (!Parallel)
            {
                return ExecuteSync(testMethod);
            }

            var files = GetData();
            var taskList = files.ToList();
            var result = new TestResult[taskList.Count];


            watch.Start();
            AsyncPump.Run(async () =>
            {
                var tasks = taskList.Select(x => Task.Run(() => RunAsyncTest(x))).ToList();
                var r = await Task.WhenAll(tasks);
                int resultIndex = 0;
                // display errors first
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
            // return dir1.EnumerateFiles("*.js", new EnumerationOptions { RecurseSubdirectories = true });
            return dir1.EnumerateFiles("*.js", SearchOption.AllDirectories);
        }

        protected virtual Task EvaluateAsync(JSContext context, string content, string fullName)
        {
            return CoreScript.EvaluateAsync(content, fullName, saveLambda ? AssemblyCodeCache.Instance : DictionaryCodeCache.Current);
            
        }

        protected virtual JSContext CreateContext(FileInfo file, SynchronizationContext ctx)
        {
            return new JSTestContext(ctx);
        }

        [HandleProcessCorruptedStateExceptions]
        protected async Task<TestResult> RunAsyncTest(FileInfo file)
        {
            // var watch = new Stopwatch();
            // watch.Start();
            var old = SynchronizationContext.Current;
            try
            {
                var ctx = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(ctx);
                var start = watch.ElapsedTicks;
                Exception lastError = null;
                Debug.WriteLine($"Processing {file.FullName}");
                StringBuilder sb = new StringBuilder();
                try
                {
                    string content;
                    using (var fs = file.OpenText())
                    {
                        content = await fs.ReadToEndAsync();
                    }
                    using (var jc = CreateContext(file, ctx))
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
                        await EvaluateAsync(jc, content, file.FullName);
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
            } finally {
                SynchronizationContext.SetSynchronizationContext(old);
            }
        }

    }
}
