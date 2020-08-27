using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Generator
{
    [TestClass]
    public class FilesTest
    {

        [TestMethod]
        public Task TestsAsync()
        {
            DirectoryInfo files = new DirectoryInfo("../../../Generator/Files");

            return ExecuteDirectoryAsync(files);
        }
        async Task ExecuteDirectoryAsync(DirectoryInfo files)
        {
            var tasks = files.EnumerateFiles().Select(x => TestFile(x)).ToArray();
            var dirs = files.EnumerateDirectories().Select(x => ExecuteDirectoryAsync(x)).ToArray();
            await Task.WhenAll(tasks);
            await Task.WhenAll(dirs);
        }
        async Task TestFile(FileInfo x)
        {
            var jc = new JSContext();
            var content = await File.ReadAllTextAsync(x.FullName);
            try
            {
                var value = CoreScript.Evaluate(content);
                Assert.IsTrue(value.BooleanValue);
            }catch (Exception ex)
            {
                throw new TestPlatformException($"Failed: {x.FullName}\r\n{ex}", ex);
            }
        }

    }
}
