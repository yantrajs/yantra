using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Utils { 
partial class JSAssert {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSAssert.Assert(in a)
                            , "Assert"
                            , "function Assert() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.Assert] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context[KeyString.Function] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting StrictEqual as strictEqual
@class.FastAddValue(KeyString.strictEqual, new JSFunction(context, JSAssert.StrictEqual, "strictEqual" ,"function strictEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting NotStrictEqual as notStrictEqual
@class.FastAddValue(KeyString.notStrictEqual, new JSFunction(context, JSAssert.NotStrictEqual, "notStrictEqual" ,"function notStrictEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Equal as equal
@class.FastAddValue(KeyString.equal, new JSFunction(context, JSAssert.Equal, "equal" ,"function equal() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting DoubleEqual as doubleEqual
@class.FastAddValue(KeyString.doubleEqual, new JSFunction(context, JSAssert.DoubleEqual, "doubleEqual" ,"function doubleEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting NotEqual as notEqual
@class.FastAddValue(KeyString.notEqual, new JSFunction(context, JSAssert.NotEqual, "notEqual" ,"function notEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Throws as throws
@class.FastAddValue(KeyString.throws, new JSFunction(context, JSAssert.Throws, "throws" ,"function throws() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Fail as fail
@class.FastAddValue(KeyString.fail, new JSFunction(context, JSAssert.Fail, "fail" ,"function fail() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Match as match
@class.FastAddValue(KeyString.match, new JSFunction(context, JSAssert.Match, "match" ,"function match() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
context.Assert_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
