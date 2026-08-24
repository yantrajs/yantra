using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace YantraJS.JSClassGenerator;

internal class Program
{
    static void Main(string[] args)
    {
        var argText = System.Text.Json.JsonSerializer.Serialize(args);
        Console.WriteLine($"Args: {argText}");

        var path = args[0];

        var root = args[1];

        var sgc = new SourceGeneratorContext();
        sgc.AddFiles(new DirectoryInfo(path));

        sgc.GenerateSource(root);
    }

}
