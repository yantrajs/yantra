using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Typed { 
partial class JSUint8ClampedArray {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSUint8ClampedArray(in a)
                            , "Uint8ClampedArray"
                            , "function Uint8ClampedArray() { [native code] }"
                            , length:3);
                        if (register) {
                            context[KeyString.Uint8ClampedArray] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context[KeyString.TypedArray] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting BYTES_PER_ELENENT as BYTES_PER_ELEMENT
@class.FastAddValue(
                KeyString.BYTES_PER_ELEMENT,
                ClrProxy.Marshal(JSUint8ClampedArray.BYTES_PER_ELENENT),
                JSPropertyAttributes.ReadonlyValue);
// Exporting From as from
@class.FastAddValue(KeyString.from, new JSFunction(context, JSUint8ClampedArray.From, "from" ,"function from() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Of as of
@class.FastAddValue(KeyString.of, new JSFunction(context, JSUint8ClampedArray.Of, "of" ,"function of() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
context.Uint8ClampedArray_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
