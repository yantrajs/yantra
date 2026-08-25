using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSError {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSError(in a)
                            , "Error"
                            , "function Error() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.Error] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting ToString as toString
prototype.FastAddValue(KeyString.toString, new JSFunction(context, (in Arguments a) =>
                    a.This is JSError @this
                        ? @this.ToString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSError")
                        , "toString"
                        ,"function toString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
context.Error_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
