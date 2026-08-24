using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Core.Error { 
partial class JSSuppressedError {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSSuppressedError(in a)
                            , "SuppressedError"
                            , "function SuppressedError() { [native code] }"
                            );
                        if (register) {
                            context["SuppressedError".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["Error".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting Error as error
prototype.FastAddProperty(
                "error".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSSuppressedError @this
                        ? @this.Error
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSSuppressedError") ,
                "get error"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSSuppressedError @this) {
                         @this.Error = a[0];
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSSuppressedError");
                    return JSUndefined.Value;
                },
                "set error"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Suppressed as suppressed
prototype.FastAddProperty(
                "suppressed".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSSuppressedError @this
                        ? @this.Suppressed
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSSuppressedError") ,
                "get suppressed"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSSuppressedError @this) {
                         @this.Suppressed = a[0];
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSSuppressedError");
                    return JSUndefined.Value;
                },
                "set suppressed"),
                JSPropertyAttributes.ConfigurableProperty);
return @class;
}
}
}
