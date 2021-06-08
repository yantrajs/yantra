using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                var file = new FileInfo(Environment.CommandLine);
                executable = Path.Combine( file.Directory.FullName, $"{System.IO.Path.GetFileNameWithoutExtension( file.Name)}.exe");
                var now = DateTime.Now;
                output = new FileInfo( Path.Combine(test262, $"{now.Year}-{now.Month:D2}-{now.Day:D2}-{now.Hour:D2}{now.Minute:D2}{now.Second:D2}.html"));
                using (var outputStream = output.OpenWrite())
                {
                    var sr = new StreamWriter(outputStream);
                    sr.WriteLine("<html><body>");
                    sr.Flush();
                    await RunTests(language, sr);
                }

                return;
            }

            // run tests...
            AsyncPump.Run(() => RunJSTest(args[0]));
        }

        private static async Task RunJSTest(string filePath)
        {
            using (var js = new JSContext())
            {
                var text = await System.IO.File.ReadAllTextAsync(filePath);

                // include harness...
                await EvaluateAsync(Path.Combine(harnessFolder.FullName, "assert.js"), js);
                await EvaluateAsync(Path.Combine(harnessFolder.FullName, "sta.js"), js);

                Environment.ExitCode = 0;

                // find more harness...
                try
                {
                    await CoreScript.EvaluateAsync(text, filePath);
                }
                catch (Exception ex)
                {
                    Environment.ExitCode = -1;
                    throw ex;
                }
            }
        }

        private static async Task EvaluateAsync(string v, JSContext c)
        {
            JSContext.CurrentContext = c;
            CoreScript.Evaluate(await System.IO.File.ReadAllTextAsync(v), v);
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
                return;

            string error = (string.IsNullOrWhiteSpace(r.StdErr) ?
                r.StdOut
                : $"{r.StdErr}\r\n{r.StdOut}").Trim();

            if (string.IsNullOrWhiteSpace(error))
                return;

            lock (sr)
            {


                sr.WriteLine($"<div><a href='{fileInfo.FullName}'>{fileInfo.FullName}</a></div>");
                sr.WriteLine($"<div><pre>{error}</pre></div>");
                sr.Flush();

                Console.WriteLine($"Failed: {fileInfo.FullName}");
                Console.WriteLine($"{error}");
            }
        }

    }
}
