using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Typed { 
partial class JSInt32Array {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSInt32Array(in a)
                            , "Int32Array"
                            , "function Int32Array() { [native code] }"
                            , length:3);
                        if (register) {
                            context["Int32Array".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["TypedArray".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting BYTES_PER_ELENENT as BYTES_PER_ELEMENT
@class.FastAddValue(
                "BYTES_PER_ELEMENT".ToKeyString(),
                ClrProxy.Marshal(JSInt32Array.BYTES_PER_ELENENT),
                JSPropertyAttributes.ReadonlyValue);
// Exporting From as from
@class.FastAddValue("from".ToKeyString(), new JSFunction(context, JSInt32Array.From, "from" ,"function from() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Of as of
@class.FastAddValue("of".ToKeyString(), new JSFunction(context, JSInt32Array.Of, "of" ,"function of() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
