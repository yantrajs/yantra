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
                            context[KeyString.Float64Array] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context[KeyString.TypedArray] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting BYTES_PER_ELENENT as BYTES_PER_ELEMENT
@class.FastAddValue(
                KeyString.BYTES_PER_ELEMENT,
                ClrProxy.Marshal(JSFloat64Array.BYTES_PER_ELENENT),
                JSPropertyAttributes.ReadonlyValue);
// Exporting From as from
@class.FastAddValue(KeyString.from, new JSFunction(context, JSFloat64Array.From, "from" ,"function from() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Of as of
@class.FastAddValue(KeyString.of, new JSFunction(context, JSFloat64Array.Of, "of" ,"function of() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
context.Float64Array_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
