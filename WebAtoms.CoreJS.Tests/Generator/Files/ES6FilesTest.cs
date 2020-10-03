using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Tests.ES6
{
    public class ES6FileCollection<T>
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
            var dir = new DirectoryInfo("../../../Generator/Files/es6/" + folder);
            return GetFiles(dir, dir).Select(x => new object[] { x }).ToList();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Sent by Test Engine")]
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

    // [TestClass]
    public class ES6Syntax : ES6FileCollection<ES6Syntax>
    {
    }

}
