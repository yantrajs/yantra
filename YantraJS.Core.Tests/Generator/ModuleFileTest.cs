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
    private static readonly string folder = "../../../Generator/Modules/";

    public static IEnumerable<object[]> GetJavaScriptTestFiles()
        => FilesTest.GetJavaScriptTestFiles(folder);

    public static string GetCustomTestName(MethodInfo methodInfo, object[] data)
        => FilesTest.GetCustomTestName(folder, methodInfo, data);

    [TestMethod]
    [DynamicData(nameof(GetJavaScriptTestFiles), DynamicDataDisplayName = nameof(GetCustomTestName))]
    public async Task ExecuteScript(string filePath)
    {
        await RunAsyncTest(new FileInfo(filePath));
    }

    protected async Task RunAsyncTest(FileInfo file)
    {

        // var watch = new Stopwatch();
        // watch.Start();
        var old = SynchronizationContext.Current;
        try
        {
            var ctx = new SynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(ctx);
            string content = await System.IO.File.ReadAllTextAsync(file.FullName);
            using var m = new JSModuleContext(ctx);
            try
            {
                await m.RunAsync(file.DirectoryName, "./" + file.Name);
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
