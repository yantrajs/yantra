using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Generator
{

    [TestClass]
    public class FilesTest
    {

        public static IEnumerable<object[]> AllTests
        {
            get
            {
                DirectoryInfo files = new DirectoryInfo("../../../Generator/Files");
                return GetFiles(files, files)
                    .Select(x => new object[] { x }).ToArray();
            }
        }

        static IEnumerable<(FileInfo,string)> GetFiles(DirectoryInfo files, DirectoryInfo root)
        {
            foreach(var file in files.EnumerateFiles())
            {
                yield return (file, Path.GetRelativePath(root.FullName, file.FullName));
            }
            foreach(var dir in files.EnumerateDirectories())
            {
                foreach(var file in GetFiles(dir, root))
                {
                    yield return file;
                }
            }
        }

        public static string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var p = ((FileInfo, string))data[0];
            return p.Item2;
        }

        [TestMethod]
        [DynamicData("AllTests", DynamicDataDisplayName = "GetDisplayName")]
        public async Task TestFile((FileInfo,string) test)
        {
            var (x, name) = test;
            var content = await File.ReadAllTextAsync(x.FullName);
            var jc = new JSTestContext();
            CoreScript.Evaluate(content);
            
        }

    }
}
