#r "nuget: YantraJS.Core,1.2.1"
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YPath {

    private static KeyString root = "root";

    private static KeyString dir = "dir";

    private static KeyString @base = "base";

    private static KeyString ext = "ext";

    public static JSValue delimiter = new JSString(Path.PathSeparator);

    public static JSValue sep = new JSString(Path.DirectorySeparatorChar);

    public static JSValue Dirname(in Arguments a) {
        var name = a.Get1().ToString();
        var file = new FileInfo(name);
        return new JSString(file.DirectoryName);
    }

    public static JSValue Extname(in Arguments a) {
        var name = a.Get1().ToString();
        var file = new FileInfo(name);
        return new JSString(file.Extension);
    }

    public static JSValue IsAbsolute(in Arguments a) {
        var name = a.Get1().ToString();
        return Path.IsPathRooted(name) ? JSBoolean.True : JSBoolean.False;
    }

    public static JSValue Join(in Arguments a) {
        string path = null;
        for(var i = 0; i < a.Length; i++) {
            if(!(a.GetAt(i) is JSString @string))
                throw JSContext.CurrentContext.NewTypeError($"input must be a string");
            path = path == null ? path : Path.Join(path, @string.ToString());
        }
        return new JSString( path ?? ".");
    }

    public static JSValue Normalize(in Arguments a) {
        return new JSString(Path.GetFullPath(a.Get1().ToString()));
    }

    public static JSValue Parse(in Arguments a) {
        if(!(a.Get1() is JSString path))
            throw JSContext.CurrentContext.NewTypeError($"Path must be a string");
        var file = new FileInfo(path.ToString());
        var name = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
        var @object = new JSObject();
        @object[dir] = new JSString(file.DirectoryName);
        @object[root] = new JSString(file.Directory.Root.FullName);
        @object[@base] = new JSString(file.Name);
        @object[KeyStrings.name] = new JSString(name);
        @object[ext] = new JSString(file.Extension);
        return @object;
    }

    public static JSValue Resolve(in Arguments a) {
        return Join(a);
    }

}

static Task Module(JSModule module) {
    module.Exports = ClrType.From(typeof(YPath));
    return Task.CompletedTask;
}

return (JSModuleDelegate)Module;