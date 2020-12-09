#r "nuget: YantraJS.Core,1.0.14"
using System;
using System.Linq;
using YantraJS.Core;
using YantraJS.Core.Clr;

public static class FSDir {

}

public static class FSModule {

    // public static JSValue Dir = ClrType.From(typeof(FSDir));

    public static JSValue Stat(in Arguments a) {
        var (path, options) = a.Get2();
        return JSUndefined.Value;
    }

    public static JSValue Access(in Arguments a) {
        throw JSContext.CurrentContext.NewTypeError($"Not implemented");
    }

}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {

    module["exports"] = ClrType.From(typeof(FSModule));

}

return (JSModuleDelegate)Module;