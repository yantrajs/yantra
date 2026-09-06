using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Emit;
using YantraJS.Utils;

namespace YantraJS.Core.FileTests;

[TestClass]
public class ModuleFileTest
{
    private static readonly string folder = "../../../Modules/";

    private static readonly DirectoryInfo rootModules
        = new DirectoryInfo("../../../../modules/inbuilt");

    private static IEnumerable<object[]> GetModules(DirectoryInfo dir)
    {
        var indexFile = new FileInfo(dir.FullName + "/index.js");
        if (indexFile.Exists)
        {
            yield return new object[] { indexFile.FullName };
        }
        else
        {
            foreach (var d in dir.EnumerateDirectories())
            {
                foreach(var c in GetModules(d))
                {
                    yield return c;
                }
            }
        }
    }

    public static IEnumerable<object[]> GetJavaScriptTestFiles()
    {
        return GetModules(new DirectoryInfo(folder));
    }

    public static string GetCustomTestName(MethodInfo methodInfo, object[] data)
        => FilesTest.GetCustomTestName(folder, methodInfo, data);

    [TestMethod]
    [DynamicData(nameof(GetJavaScriptTestFiles), DynamicDataDisplayName = nameof(GetCustomTestName))]
    public async Task ExecuteScript(string filePath)
    {
        await RunAsyncTest(new FileInfo(filePath));
    }

    protected async Task RunAsyncTest(FileInfo fileInfo)
    {

        // var watch = new Stopwatch();
        // watch.Start();
        var old = SynchronizationContext.Current;
        try
        {
            var ctx = new SynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(ctx);
            string content = await System.IO.File.ReadAllTextAsync(fileInfo.FullName);
            using var m = new YantraContext(fileInfo.DirectoryName, ctx);
            try
            {
                await m.RunAsync(fileInfo.DirectoryName, "./" + fileInfo.Name, new string[] {
                    rootModules.FullName,
                    rootModules.FullName + "/bin"
                });
            }
            catch (TaskCanceledException)
            {

            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(old);
        }
    }

}
