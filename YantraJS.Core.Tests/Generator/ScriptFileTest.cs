using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Emit;
using YantraJS.Tests;
using YantraJS.Utils;

namespace YantraJS.Core.ScriptTests;

[TestClass]
public class ScriptFileTest
{
    private static readonly string folder = "../../../Generator/Files/";

    public static IEnumerable<object[]> GetJavaScriptTestFiles()
    {
        // Return the full file path to the test method
        return Directory.GetFiles(folder, "*.js", SearchOption.AllDirectories)
                        .Select(filePath => new object[] {
                            filePath
                        });
    }

    public static string GetCustomTestName(MethodInfo methodInfo, object[] data)
    {
        if (data != null && data.Length > 0 && data[0] is string filePath)
        {
            // Returns just "my-script.js" or "folder_subfolder_script.js" 
            var name = Path.GetRelativePath(folder, filePath)
                    .Replace("\\", "/");
            return name;
        }
        return methodInfo.Name;
    }

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
            Exception lastError = null;
            System.Diagnostics.Debug.WriteLine($"Processing {file.FullName}");
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
                        var text = s.ToDetailString();
                        Console.WriteLine(text);
                    };
                    jc.Error += (_, e) => lastError = e;
                    await EvaluateAsync(jc, content, file.FullName);
                }
            }
            catch (Exception ex)
            {
                lastError = ex;
            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(old);
        }
    }

    protected virtual JSContext CreateContext(FileInfo file, SynchronizationContext ctx)
    {
        return new JSTestContext(ctx);
    }

    protected virtual Task EvaluateAsync(JSContext context, string content, string fullName)
    {
        return CoreScript.EvaluateAsync(content, fullName, DictionaryCodeCache.Current);

    }
}
