#r "nuget: YantraJS.Core,1.2.1"
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YantraJS;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YInspect {



}

public class YUtil {

    public static JSValue inspect = ClrType.From(typeof(YInspect));

    public static JSValue inherits(JSValue child, JSValue parent) {

        var context = JSContext.CurrentContext;
        var @object = context[KeyString.Object];

        @object.InvokeMethod("setPrototypeOf", child, parent);
        if (parent == JSNull.Value) {
            child[KeyString.prototype] = @object.InvokeMethod("create", parent);
        } else {
            var __ = new JSFunction((in Arguments a) => {
                a.This[KeyString.constructor] = child;
                return a.This;
            });
            __[KeyString.prototype] = child[KeyString.prototype];
            child[KeyString.prototype] = __.CreateInstance(Arguments.Empty);
        }

        return JSUndefined.Value;
    }


}

static Task Module(JSModule module) {
    module.Exports = ClrType.From(typeof(YUtil));
    return Task.CompletedTask;
}


return (JSModuleDelegate)Module;