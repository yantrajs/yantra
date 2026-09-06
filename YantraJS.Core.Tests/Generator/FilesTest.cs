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

namespace YantraJS.Core.FileTests;

public static class FilesTest
{
    public static IEnumerable<object[]> GetJavaScriptTestFiles(string folder)
    {
        ILCodeGenerator.GenerateLogs = true;

        // Return the full file path to the test method
        return Directory
            .GetFiles(folder, "*.js", SearchOption.AllDirectories)
            .Select(filePath => new object[] {
                filePath
            });
    }

    public static string GetCustomTestName(string folder, MethodInfo methodInfo, object[] data)
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

}
