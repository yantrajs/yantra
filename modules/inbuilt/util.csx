#r "nuget: YantraJS.Core,1.0.18"
using System;
using System.IO;
using System.Linq;
using YantraJS;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YInspect {



}

public class YUtil {

    public static JSValue inspect = ClrType.From(typeof(YInspect));

    public static JSValue inherits(JSValue child, JSValue parent) {

        var context = JSContext.CurrentContext;
        var @object = context[KeyStrings.Object];

        @object.InvokeMethod("setPrototypeOf", child, parent);
        if (parent == JSNull.Value) {
            child[KeyStrings.prototype] = @object.InvokeMethod("create", parent);
        } else {
            var __ = new JSFunction((in Arguments a) => {
                a.This[KeyStrings.constructor] = child;
                return a.This;
            });
            __[KeyStrings.prototype] = child[KeyStrings.prototype];
            child[KeyStrings.prototype] = __.CreateInstance(Arguments.Empty);
        }

        return JSUndefined.Value;
    }


}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {
    module["exports"] = ClrType.From(typeof(YUtil));
}


return (JSModuleDelegate)Module;