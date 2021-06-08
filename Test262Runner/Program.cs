using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Test262Runner
{
    class Program
    {
        static DirectoryInfo harnessFolder;

        static DirectoryInfo language;

        static DirectoryInfo builtins;

        static DirectoryInfo root;

        static DirectoryInfo output;

        static string executable;

        static void Main(string[] args)
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
                executable = Process.GetCurrentProcess().StartInfo.FileName;

                RunTests(language);

                return;
            }
        }

        private static void RunTests(DirectoryInfo language)
        {
            foreach(var file in language.EnumerateFileSystemInfos())
            {
                if(file is DirectoryInfo di)
                {
                    RunTests(di);
                    return;
                }

                RunTest(file as FileInfo);
            }
        }

        private static void RunTest(FileInfo fileInfo)
        {
            var process = Process.Start(executable, fileInfo.FullName);
            process.WaitForExit(60 * 60 * 2);
        }
    }
}
