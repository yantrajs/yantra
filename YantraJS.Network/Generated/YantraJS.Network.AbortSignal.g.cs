using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class AbortSignal {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new AbortSignal(in a)
                            , "AbortSignal"
                            , "function AbortSignal() { [native code] }"
                            );
                        if (register) {
                            context["AbortSignal".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["EventTarget".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting Aborted as aborted
prototype.FastAddProperty(
                "aborted".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is AbortSignal @this
                        ? ClrProxy.Marshal(@this.Aborted)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to AbortSignal") ,
                "get aborted"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is AbortSignal @this) {
                         @this.Aborted = JSValueToClrConverter.ToBoolean(a[0], "aborted");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to AbortSignal");
                    return JSUndefined.Value;
                },
                "set aborted"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Reason as reason
prototype.FastAddProperty(
                "reason".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is AbortSignal @this
                        ? ClrProxy.Marshal(@this.Reason)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to AbortSignal") ,
                "get reason"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is AbortSignal @this) {
                         @this.Reason = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "reason");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to AbortSignal");
                    return JSUndefined.Value;
                },
                "set reason"),
                JSPropertyAttributes.ConfigurableProperty);
return @class;
}
}
}
