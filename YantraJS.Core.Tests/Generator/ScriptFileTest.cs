using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Emit;
using YantraJS.Generator;
using YantraJS.Utils;

namespace YantraJS.Core.FileTests;

[TestClass]
public class ScriptFileTest
{
    private static readonly string folder = "../../../Generator/Files/";

    public static IEnumerable<object[]> GetJavaScriptTestFiles()
        => FilesTest.GetJavaScriptTestFiles(folder);

    public static string GetCustomTestName(MethodInfo methodInfo, object[] data)
        => FilesTest.GetCustomTestName(folder, methodInfo, data);

    [TestMethod]
    [DynamicData(nameof(GetJavaScriptTestFiles), DynamicDataDisplayName = nameof(GetCustomTestName))]
    public async Task ExecuteScript(string filePath)
    {
        ILCodeGenerator.GenerateLogs = true;
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
            Exception lastError = null;
            string content = await System.IO.File.ReadAllTextAsync(file.FullName);
            using (var jc = new JSTestContext(ctx))
            {
                jc.Log += (_, s) =>
                {
                    var text = s.ToDetailString();
                    Console.WriteLine(text);
                };
                jc.Error += (_, e) => lastError = e;
                await CoreScript.EvaluateAsync(content, file.FullName, DictionaryCodeCache.Current);
            }
            if (lastError != null)
            {
                throw JSException.From(lastError);
            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(old);
        }
    }

}
