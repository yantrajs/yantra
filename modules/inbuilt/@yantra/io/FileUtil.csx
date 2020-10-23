#r "nuget: YantraJS.Core, 1.0.1-CI-20201023-094851"
using System;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Clr;

public class FileUtil {

    public static JSValue WriteAllText(in Arguments a) {
        try {
            var (filePath, contents, encoding) = a.Get3();
            if (encoding.IsUndefined) {
                System.IO.File.WriteAllText(filePath.ToString(), contents.ToString());
                return JSUndefined.Value;
            }
            System.IO.File.WriteAllText(filePath.ToString(), contents.ToString(), System.Text.Encoding.GetEncoding(encoding.ToString()));
            return JSUndefined.Value;
        } catch (Exception ex) {
            throw JSContext.Current.NewError(ex.Message);
        }
    }

    public static JSValue ReadAllText(in Arguments a) {
        try {
            var (filePath, encoding) = a.Get2();
            string contents;
            if (encoding.IsUndefined) {
                contents = System.IO.File.ReadAllText(filePath.ToString());
            } else {
                contents = System.IO.File.ReadAllText(filePath.ToString(), System.Text.Encoding.GetEncoding(encoding.ToString()));
            }
            return new JSString(contents);
        } catch (Exception ex) {
            throw JSContext.Current.NewError(ex.Message);
        }
    }
}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {

    exports["default"] = ClrType.From(typeof(FileUtil));

}

return (JSModuleDelegate)Module;
