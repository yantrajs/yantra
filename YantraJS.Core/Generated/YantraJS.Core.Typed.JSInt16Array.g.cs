using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Typed { 
partial class JSInt16Array {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSInt16Array(in a)
                            , "Int16Array"
                            , "function Int16Array() { [native code] }"
                            , length:3);
                        if (register) {
                            context["Int16Array".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["TypedArray".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting BYTES_PER_ELENENT as BYTES_PER_ELEMENT
@class.FastAddValue(
                "BYTES_PER_ELEMENT".ToKeyString(),
                ClrProxy.Marshal(JSInt16Array.BYTES_PER_ELENENT),
                JSPropertyAttributes.ReadonlyValue);
// Exporting From as from
@class.FastAddValue("from".ToKeyString(), new JSFunction(context, JSInt16Array.From, "from" ,"function from() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Of as of
@class.FastAddValue("of".ToKeyString(), new JSFunction(context, JSInt16Array.Of, "of" ,"function of() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
