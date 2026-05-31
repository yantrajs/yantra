#r "nuget: YantraJS.Core,1.2.1"
using System;
using System.Linq;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.Clr;
using System.Runtime.InteropServices;

public class YProcess {

    public static JSValue Argv;

    public static JSValue Arch;

    public static JSValue Platform;

}

static Task Module(JSModule module) {
    var arch = RuntimeInformation.ProcessArchitecture.ToString();
    YProcess.Arch = arch == null ? JSNull.Value : new JSString(arch);
    YProcess.Platform = new JSString(System.Environment.OSVersion.Platform.ToString());
    var args = System.Environment.GetCommandLineArgs();
    YProcess.Argv = new JSArray( args.Select(x => new JSString(x)));
    module.Exports = ClrType.From(typeof(YProcess));
    return Task.CompletedTask;
}

return (JSModuleDelegate)Module;