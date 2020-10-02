using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Utils;
namespace WebAtoms.CoreJS.Tests.Generator
{
    public class FileCollection<T>
    {
        public static IEnumerable<object[]> AllTests
            => GetFileCollection(typeof(T).Name);

        public static IEnumerable<object[]> GetFileCollection(string folder)
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
            var dir = new DirectoryInfo("../../../Generator/Files/es5/" + folder);
            return GetFiles(dir, dir).Select(x => new object[] { x }).ToList();
        }

        public static string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var p = ((FileInfo, string))data[0];
            return p.Item2;
        }

        [TestMethod]
        [DynamicData("AllTests", DynamicDataDisplayName = "GetDisplayName")]
        public async Task RunFile((FileInfo, string) test)
        {
            var (x, _) = test;
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
    public class Objects: FileCollection<Objects>
    {
    }

    [TestClass]
    public class Statements : FileCollection<Statements>
    {
    }

    [TestClass]
    public class Syntax : FileCollection<Syntax>
    {
    }

    [TestClass]
    public class String : FileCollection<String>
    {
    }


    [TestClass]
    public class Function : FileCollection<Function>
    {
    }

    // [TestClass]
    public class Index : FileCollection<Index>
    {
    }

}
