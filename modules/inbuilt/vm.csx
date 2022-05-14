#r "nuget: YantraJS.Core,1.2.1"
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YantraJS;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YScript {

    private string code;
    private string fileName;

    public YScript(in Arguments a) {
        code = a.Get1().ToString();
        var options = a.GetAt(1);

        if (options.IsObject) {
            var fn = options["filename"];
            if(!fn.IsUndefined) {
                fileName = fn.ToString();
            }
        }
    }

    public JSValue RunInContext(in Arguments a) {
        var (contextifiedObject, options) = a.Get2();
        var context = contextifiedObject[YVM.context];
        var oldContext = JSContext.CurrentContext;
        try {
            JSContext.CurrentContext = context as JSContext;
            return CoreScript.Evaluate(code, fileName);
        } finally {
            JSContext.CurrentContext = oldContext;
        }
    }

    public JSValue RunInNewContext(JSValue contextObject, JSValue options) {
        contextObject = contextObject.IsUndefined ? new JSObject() : contextObject;
        // check if it is already a context...
        if (!(contextObject[YVM.context] is JSContext)) {
            YVM.CreateContext(new Arguments(JSUndefined.Value, contextObject));
        }
        return RunInContext(new Arguments(JSUndefined.Value, contextObject));
    }

    public JSValue RunInThisContext(JSValue options) {
        return CoreScript.Evaluate(code, fileName);
    }    

}

public class YVM {

    internal static JSSymbol context;

    public static JSValue Script = ClrType.From(typeof(YScript));

    public static JSValue IsContext(in Arguments a) {
        var a1 = a.Get1();
        var c = a1[context];
        return c.IsUndefined ? JSBoolean.True : JSBoolean.False;
    }    

    public static JSValue CreateContext(in Arguments a) {
        var a1 = a.Get1();
        if (a1.IsUndefined || a1.IsNull || (!a1.IsObject))
            throw JSContext.CurrentContext.NewTypeError($"Object to contextify must be an object");
        var old = JSContext.CurrentContext;
        var c = new JSContext();
        JSContext.CurrentContext = old;
        a1[context] = c;
        return a1;
    }

    public static JSValue RunInContext(in Arguments a) {
        var (code, contextifiedObject, options) = a.Get3();
        var script = new YScript(new Arguments(code, options));
        return script.RunInContext(new Arguments(contextifiedObject, options));
    }

    public static JSValue RunInNewContext(in Arguments a) {
        var (code, contextObject, options) = a.Get3();
        var script = new YScript(new Arguments(code, options));
        return script.RunInNewContext(contextObject, options);
    }

    public static JSValue RunInThisContext(in Arguments a) {
        var (code, options) = a.Get2();
        var script = new YScript(new Arguments(code, options));
        return script.RunInThisContext(options);
    }
}

static Task Module(JSModule module) {
    YVM.context = YVM.context ?? new JSSymbol("context");
    module.Exports = ClrType.From(typeof(YVM));
    return Task.CompletedTask;
}


return (JSModuleDelegate)Module;