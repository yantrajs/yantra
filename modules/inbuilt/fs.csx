#r "nuget: YantraJS.Core, 1.0.1-CI-20201023-094851"
using System;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Clr;

public static class FSDir {

}

public static class FSModule {

    // public static JSValue Dir = ClrType.From(typeof(FSDir));

    public static JSValue Stat(in Arguments a) {
        var (path, options) = a.Get2();
        return JSUndefined.Value;
    }

}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {

    module["exports"] = ClrType.From(typeof(FSModule));

}

return (JSModuleDelegate)Module;