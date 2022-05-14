#r "nuget: YantraJS.Core,1.2.1"
using System;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class FileUtil {

    public static JSValue WriteAllText(in Arguments a) {
        try {
            var (a1, a2, a3) = a.Get3();
            var filePath = a1.ToString();
            var contents = a2.ToString();
            var encoding = a3.AsStringOrDefault("utf-8");
            System.IO.File.WriteAllText(filePath, contents, System.Text.Encoding.GetEncoding(encoding));
            return JSUndefined.Value;
        } catch (Exception ex) {
            throw JSContext.CurrentContext.NewError(ex.Message);
        }
    }

    public static JSValue ReadAllText(in Arguments a) {
        try {
            var (a1, a2) = a.Get2();
            var filePath = a1.ToString();
            var encoding = a2.AsStringOrDefault("utf-8");
            var contents = System.IO.File.ReadAllText(filePath, System.Text.Encoding.GetEncoding(encoding));
            return new JSString(contents);
        } catch (Exception ex) {
            throw JSContext.CurrentContext.NewError(ex.Message);
        }
    }
}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {

    exports["default"] = ClrType.From(typeof(FileUtil));

}

return (JSModuleDelegate)Module;
