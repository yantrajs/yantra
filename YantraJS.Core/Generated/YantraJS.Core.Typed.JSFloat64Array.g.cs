using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Typed { 
partial class JSFloat64Array {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSFloat64Array(in a)
                            , "Float64Array"
                            , "function Float64Array() { [native code] }"
                            , length:3);
                        if (register) {
                            context["Float64Array".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["TypedArray".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting BYTES_PER_ELENENT as BYTES_PER_ELEMENT
@class.FastAddValue(
                "BYTES_PER_ELEMENT".ToKeyString(),
                ClrProxy.Marshal(JSFloat64Array.BYTES_PER_ELENENT),
                JSPropertyAttributes.ReadonlyValue);
// Exporting From as from
@class.FastAddValue("from".ToKeyString(), new JSFunction(context, JSFloat64Array.From, "from" ,"function from() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Of as of
@class.FastAddValue("of".ToKeyString(), new JSFunction(context, JSFloat64Array.Of, "of" ,"function of() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
