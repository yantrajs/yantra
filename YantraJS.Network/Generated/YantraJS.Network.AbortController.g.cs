using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class AbortController {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new AbortController(in a)
                            , "AbortController"
                            , "function AbortController() { [native code] }"
                            );
                        if (register) {
                            context["AbortController".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Abort as abort
prototype.FastAddValue("abort".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is AbortController @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to AbortController");
			var pname = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "name");
			@this.Abort(pname);
			return JSUndefined.Value;
			}
			,
                "Abort"
                ,"function abort() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
