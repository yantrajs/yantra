#r "nuget: YantraJS.Core,1.0.11"
using System;
using System.IO;
using System.Linq;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YPath {

    public static JSValue delimiter = new JSString(Path.PathSeparator);

    public static JSValue sep = new JSString(Path.DirectorySeparatorChar);

    public static JSValue dirname(in Arguments a) {
        var name = a.Get1().ToString();
        var file = new FileInfo(name);
        return new JSString(file.DirectoryName);
    }

    public static JSValue extname(in Arguments a) {
        var name = a.Get1().ToString();
        var file = new FileInfo(name);
        return new JSString(file.Extension);
    }

    public static JSValue isAbsolute(in Arguments a) {
        var name = a.Get1().ToString();
        return Path.IsPathRooted(name) ? JSBoolean.True : JSBoolean.False;
    }

    public static JSValue join(in Arguments a) {
        for(var i = 0; i < a.Length; i++) {
            var ai = a.GetAt(i);
        }
    }

}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {
    module["exports"] = ClrType.From(typeof(YPath));

}

return (JSModuleDelegate)Module;