#r "nuget: YantraJS.Core,1.0.14"
using System;
using System.IO;
using System.Linq;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YScript {

    private string code;

    public YScript(in Arguments a) {
        code = a.Get1().ToString();
    }

    public JSValue RunInContext(in Arguments a) {
        var (contextifiedObject, options) = a.Get2();
        var context = contextifiedObject[YVM.context];
        var oldContext = JSContext.CurrentContext;

        JSContext.CurrentContext = oldContext;
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
        var c = new JSContext();
        a1[context] = c;
        return a1;
    }

    public static JSValue RunInContext(in Arguments a) {
        var (code, contextifiedObject, options) = a.Get3();
        var script = new YScript(new Arguments(code));
        return script.RunInContext(new Arguments(contextifiedObject, options));
    }

}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {
    YVM.context = new JSSymbol("context");
    module["exports"] = ClrType.From(typeof(YVM));
}


return (JSModuleDelegate)Module;