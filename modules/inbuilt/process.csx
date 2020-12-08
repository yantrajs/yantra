#r "nuget: YantraJS.Core,1.0.12"
using System;
using System.Linq;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YProcess {

    public static JSValue Argv
        => new JSArray( System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments.Split(' ').Select(x => new JSString(x)));

    public static JSValue Arch
        => new JSString(System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));

    public static JSValue Platform
        => new JSString(System.Environment.OSVersion.Platform.ToString());

}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {

    module["exports"] = ClrType.From(typeof(YProcess));

}

return (JSModuleDelegate)Module;