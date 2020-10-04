using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var files = GetData();
            var result = new TestResult[files.Count];
            Parallel.ForEach(files, (a,l,i) =>
            {
                result[i] = RunTest(a);
            });
            return result;
        }

        public List<(FileInfo,string)> GetData()
        {
            var files = new List<(FileInfo,string)>();
            static void GetFiles(DirectoryInfo files, List<(FileInfo,string)> list)
            {
                foreach (var file in files.EnumerateFiles())
                {
                    var name = file.FullName;
                    if (!name.EndsWith(".js"))
                        continue;
                    list.Add((file, files.Name + "\\" + file.Name));
                }
                foreach (var dir in files.EnumerateDirectories())
                {
                    GetFiles(dir, list);
                }
            }
            var dir = new DirectoryInfo("../../../Generator/Files/" + root);
            GetFiles(dir, files);
            return files;
        }

        protected TestResult RunTest((FileInfo file, string name) testCase)
        {
            var watch = new Stopwatch();
            watch.Start();
            Exception lastError = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                string content;
                using (var fs = testCase.file.OpenText())
                {
                    content = fs.ReadToEnd();
                }
                using var jc = new JSTestContext();
                var c = new JSObject();
                c.ownProperties = new PropertySequence();
                c.ownProperties[KeyStrings.log.Key] = JSProperty.Property(new JSFunction((in Arguments a) => {
                    var text = a.Get1();
                    sb.AppendLine(text.ToDetailString());
                    return text;
                }));
                jc[KeyStrings.console] = c;
                CoreScript.Evaluate(content, testCase.file.FullName);
            } catch (Exception ex)
            {
                lastError = ex;
            }
            watch.Stop();
            return new TestResult {
                Outcome = lastError  == null ? UnitTestOutcome.Passed : UnitTestOutcome.Failed,
                DisplayName = testCase.name,
                Duration = watch.Elapsed,
                TestFailureException = lastError,
                LogOutput = sb.ToString()
            };
        }

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FolderAttribute: Attribute, ITestDataSource
    {
        readonly string root;
        public FolderAttribute(string root)
        {
            this.root = root;
        }

        public IEnumerable<object[]> GetData(MethodInfo method)
        {
            static IEnumerable<(FileInfo, string)> GetFiles(DirectoryInfo files, DirectoryInfo root)
            {
                foreach (var file in files.EnumerateFiles())
                {
                    var name = file.FullName;
                    if (!name.EndsWith(".js"))
                        continue;
                    name = name.Substring(root.FullName.Length + 1);
                    yield return (file, name);
                }
                foreach (var dir in files.EnumerateDirectories())
                {
                    foreach (var file in GetFiles(dir, root))
                    {
                        yield return file;
                    }
                }
            }
            var dir = new DirectoryInfo("../../../Generator/Files/" + root +  "/" + method.DeclaringType.Name);
            var list = GetFiles(dir, dir).Select(x => new object[] { x }).ToList();
            return list;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Sent by Test Engine")]
        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var p = ((FileInfo, string))data[0];
            return p.Item2;
        }
    }

    public abstract class FileTest
    {
        public abstract Task Run((FileInfo, string) test);

        protected async Task RunTest((FileInfo, string) test)
        {
            var (x, y) = test;
            if (y == null)
                return;
            string content;
            using (var fs = x.OpenText())
            {
                content = await fs.ReadToEndAsync();
            }
            using var jc = new JSTestContext();
            CoreScript.Evaluate(content, x.FullName);
        }

    }


    [TestClass]
    public class ES5
    {
        [TestFolder("es5\\Objects")]
        public void Objects(FileInfo test)
        {
            
        }

        [TestFolder("es5\\Statements")]
        public void Statements(FileInfo test)
        {

        }

        [TestFolder("es5\\Syntax")]
        public void Syntax(FileInfo test)
        {

        }

        [TestFolder("es5\\Function")]
        public void Function(FileInfo test)
        {

        }

    }

    [TestClass]
    public class ES6
    {
        [TestFolder("es6\\Syntax")]
        public void Syntax(FileInfo test)
        {

        }

    }

    [TestClass]
    public class String : FileTest
    {
        [TestMethod]
        [Folder("es5")]
        public override Task Run((FileInfo, string) test)
        {
            return RunTest(test);
        }
    }

    // [TestClass]
    public class Index : FileTest
    {
        [Folder("es5")]
        public override Task Run((FileInfo, string) test)
        {
            return RunTest(test);
        }
    }

}
