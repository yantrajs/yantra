using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class FormData {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new FormData(in a)
                            , "FormData"
                            , "function FormData() { [native code] }"
                            );
                        if (register) {
                            context["FormData".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["KeyValueStore".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting ToString as toString
prototype.FastAddValue("toString".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is FormData @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to FormData");
			var @return = @this.ToString();
			return ClrProxy.Marshal(@return);
			}
			,
                "ToString"
                ,"function toString() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
