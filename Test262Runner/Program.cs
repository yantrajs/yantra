using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YantraJS;
using YantraJS.Core;

namespace Test262Runner
{
    class Program
    {
        static DirectoryInfo harnessFolder;

        static DirectoryInfo language;

        static DirectoryInfo builtins;

        static DirectoryInfo root;

        static FileInfo output;

        static string executable;

        static TimeSpan Timeout = TimeSpan.FromMinutes(1);

        static int Total;
        static int Failed;
        static int Passed;

        async static Task Main(string[] args)
        {
            // find all tests...
            var pwd = new DirectoryInfo(Environment.CurrentDirectory);

            root = pwd.Parent.Parent.Parent.Parent;

            var test262 = Path.Combine(root.FullName, "test262");

            harnessFolder = new DirectoryInfo(Path.Combine(test262, "harness"));

            builtins = new DirectoryInfo(Path.Combine(test262, "test", "built-ins"));

            language = new DirectoryInfo(Path.Combine(test262, "test", "language"));



            if (args.Length == 0)
            {
                string cmd = Environment.CommandLine;

                var file = new FileInfo(cmd);
                executable = Path.Combine( file.Directory.FullName, $"{System.IO.Path.GetFileNameWithoutExtension( file.Name)}.exe");
                var now = DateTime.Now;
                // output = new FileInfo( Path.Combine(root.FullName,"tr", $"{now.Year}-{now.Month:D2}-{now.Day:D2}-{now.Hour:D2}{now.Minute:D2}{now.Second:D2}.html"));
                output = new FileInfo(Path.Combine(root.FullName, "tr", $"TestResult.html"));
                if (!output.Directory.Exists)
                    output.Directory.Create();
                using (var outputStream = output.OpenWrite())
                {
                    var sr = new StreamWriter(outputStream);
                    sr.WriteLine("<html><body>");
                    sr.Flush();
                    await RunTests(language, sr);
                }

                return;
            }

            string filePath = args[0];
            if (filePath == "--ask")
            {
                filePath = Console.ReadLine().Trim();
                if (filePath.StartsWith("file:///"))
                {
                    filePath = filePath.Substring(8);
                }
            }


            // run tests...
            AsyncPump.Run(() => RunJSTest(filePath));
        }

        private static async Task RunJSTest(string filePath)
        {
            using (var js = new JSContext())
            {
                var code = await System.IO.File.ReadAllTextAsync(filePath);

                // include harness...
                await EvaluateAsync(Path.Combine(harnessFolder.FullName, "assert.js"), js);
                await EvaluateAsync(Path.Combine(harnessFolder.FullName, "sta.js"), js);

                js["$DONOTEVALUATE"] = new JSFunction((in Arguments a) => {
                    return JSUndefined.Value;
                });

                Environment.ExitCode = 0;

                var config = Config.Parse(code);
                if (config.Ignore)
                    return;
                if(config.Includes != null)
                {
                    foreach(var item in config.Includes)
                    {
                        await EvaluateAsync(Path.Combine(harnessFolder.FullName, item), js);
                    }
                }
                if (config.Negative != null)
                {
                    try
                    {
                        CoreScript.Evaluate(code, filePath);
                        throw new Exception($"Exception not thrown");
                    }
                    catch (JSException)
                    {
                        return;
                    }
                }


                // find more harness...
                try
                {
                    await CoreScript.EvaluateAsync(code, filePath);
                }
                catch (Exception ex)
                {
                    Environment.ExitCode = -1;
                    // throw ex;
                    Console.Error.WriteLine(ex.ToString());
                }

                // YDispatcher.Shutdown();
            }
        }

        private static async Task EvaluateAsync(string v, JSContext c)
        {
            JSContext.CurrentContext = c;
            CoreScript.Evaluate(await System.IO.File.ReadAllTextAsync(v),v);
        }

        private static async Task RunTests(DirectoryInfo folder, StreamWriter sr)
        {
            var files = new List<Task>();
            foreach(var file in folder.EnumerateFiles("*.js"))
            {
                files.Add(RunTest(file, sr));
                // await RunTest(file, sr);
            }

            await Task.WhenAll(files);

            foreach(var dir in folder.EnumerateDirectories())
            {
                await RunTests(dir, sr);
            }
        }

        private static async Task RunTest(FileInfo fileInfo, StreamWriter sr)
        {
            var r = await AsyncProcess.RunProcessAsync(new ProcessStartInfo
            {
                FileName = executable,
                Arguments = fileInfo.FullName,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            }, null, Timeout);

            if (r.ExitCode == 0)
            {
                lock (sr)
                {
                    Total++;
                    Passed++;
                    ConsoleHelper.WriteLineOnTop($"Total: {Total}, Passed: {Passed} Failed: {Failed}                                       ");
                }
                return;
            }

            string error = (string.IsNullOrWhiteSpace(r.StdErr) ?
                r.StdOut
                : $"{r.StdErr}\r\n{r.StdOut}").Trim();

            if (string.IsNullOrWhiteSpace(error))
                return;

            lock (sr)
            {
                Total++;
                Failed++;


                sr.WriteLine($"<div><a href='{fileInfo.FullName}'>{fileInfo.FullName}</a></div>");
                sr.WriteLine($"<div><pre>{error}</pre></div>");
                sr.Flush();

                //Console.WriteLine($"Failed: {fileInfo.FullName}");
                //Console.WriteLine($"{error}");

                ConsoleHelper.WriteLineOnTop($"Total: {Total}, Passed: {Passed} Failed: {Failed}                                       ");
            }
        }

    }
}
