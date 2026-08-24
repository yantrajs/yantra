using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSBoolean {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSBoolean.Constructor(in a)
                            , "Boolean"
                            , "function Boolean() { [native code] }"
                            );
                        if (register) {
                            context["Boolean".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting ToString as toString
prototype.FastAddValue("toString".ToKeyString(), new JSFunction(context, JSBoolean.ToString, "toString" ,"function toString() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ValueOf as valueOf
prototype.FastAddValue("valueOf".ToKeyString(), new JSFunction(context, JSBoolean.ValueOf, "valueOf" ,"function valueOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue("toLocaleString".ToKeyString(), new JSFunction(context, JSBoolean.ToLocaleString, "toLocaleString" ,"function toLocaleString() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
