#r "..\..\WebAtoms.CoreJS\bin\Debug\netstandard2.0\WebAtoms.CoreJS.dll"
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

void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {

    module["exports"] = ClrType.From(typeof(FSModule));

}