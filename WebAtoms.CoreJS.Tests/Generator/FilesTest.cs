using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Utils;
namespace WebAtoms.CoreJS.Tests.Generator
{
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
    public class Objects: FileTest
    {
        [Folder("es5")]
        [TestMethod]
        public override Task Run((FileInfo,string) test)
        {
            return RunTest(test);
        }   
    }

    [TestClass]
    public class Statements : FileTest
    {
        [Folder("es5")]
        [TestMethod]
        public override Task Run((FileInfo, string) test)
        {
            return RunTest(test);
        }
    }

    [TestClass]
    public class Syntax : FileTest
    {
        [Folder("es5")]
        [TestMethod]
        public override Task Run((FileInfo, string) test)
        {
            return RunTest(test);
        }

        [Folder("es6")]
        [TestMethod]
        public Task RunES6((FileInfo, string) test)
        {
            return RunTest(test);
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


    [TestClass]
    public class Function : FileTest
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
