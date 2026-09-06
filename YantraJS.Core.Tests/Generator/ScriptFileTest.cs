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
using YantraJS.Generator;
using YantraJS.Tests;
using YantraJS.Utils;

namespace YantraJS.Core.ScriptTests;

[TestClass]
public class ScriptFileTest
{
    private static readonly string folder = "../../../Generator/Files/";

    public static IEnumerable<object[]> GetJavaScriptTestFiles()
    {
        ILCodeGenerator.GenerateLogs = true;

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
