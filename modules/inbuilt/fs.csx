using System;
using WebAtoms.CoreJS.Core;

public static class FSDir {

}

public static class FSModule {

    // public static JSValue Dir = ClrType.From(typeof(FSDir));

    public static JSValue Stat(in Arguments a) {
        var (path, options) = a.Get2();
    }

}

void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {

    module["exports"] = ClrType.From(typeof(FSModule));

}