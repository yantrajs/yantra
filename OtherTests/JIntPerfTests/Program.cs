using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YantraJS.Core.FastParser;

public class Program
{

    private static readonly Dictionary<string, string> _files = new()
    {
        { "array-stress", null },
        { "evaluation", null },
        { "linq-js", null },
        { "minimal", null },
        { "stopwatch", null },
        { "dromaeo-3d-cube", null },
        { "dromaeo-core-eval", null },
        { "dromaeo-object-array", null },
        { "dromaeo-object-regexp", null },
        { "dromaeo-object-string", null },
        { "dromaeo-string-base64", null },
    };

    private static readonly string _dromaeoHelpers = @"
        var startTest = function () { };
        var test = function (name, fn) { fn(); };
        var endTest = function () { };
        var prep = function (fn) { fn(); };";


    public static void Main()
    {

        var files = _files.Keys.ToList();

        foreach(var fileName in files)
        {
            var script = File.ReadAllText($"Scripts/{fileName}.js");
            if (fileName.Contains("dromaeo"))
            {
                script = _dromaeoHelpers + Environment.NewLine + Environment.NewLine + script;
            }
            _files[fileName] = script;
        }

        foreach (var item in files)
        {
            var content = _files[item];
            var engine = new YantraJS.Core.JSContext();
            // By default YantraJS is strict mode only, in strict mode
            // unless you pass `this`, `this` in a global context is undefined.
            engine.Eval(content, item + ".js", engine);
        }
    }
}

