using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSFunction {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSFunction.Constructor(in a)
                            , "Function"
                            , "function Function() { [native code] }"
                            );
                        if (register) {
                            context["Function".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting ValueOf as valueOf
prototype.FastAddValue("valueOf".ToKeyString(), new JSFunction(context, JSFunction.ValueOf, "valueOf" ,"function valueOf() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Call as call
prototype.FastAddValue("call".ToKeyString(), new JSFunction(context, JSFunction.Call, "call" ,"function call() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Apply as apply
prototype.FastAddValue("apply".ToKeyString(), new JSFunction(context, JSFunction.Apply, "apply" ,"function apply() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Bind as bind
prototype.FastAddValue("bind".ToKeyString(), new JSFunction(context, JSFunction.Bind, "bind" ,"function bind() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToString as toString
prototype.FastAddValue("toString".ToKeyString(), new JSFunction(context, JSFunction.ToString, "toString" ,"function toString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
