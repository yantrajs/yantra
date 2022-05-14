#r "nuget: YantraJS.Core,1.2.1"
using System;
using System.Linq;
using System.Threading.Tasks;
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

static Task Module(JSModule module) {

    module.Exports = ClrType.From(typeof(YProcess));
    return Task.CompletedTask;
}

return (JSModuleDelegate)Module;