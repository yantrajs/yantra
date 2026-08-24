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
                            context["Assert".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["Function".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
// Exporting StrictEqual as strictEqual
@class.FastAddValue("strictEqual".ToKeyString(), new JSFunction(context, JSAssert.StrictEqual, "strictEqual" ,"function strictEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting NotStrictEqual as notStrictEqual
@class.FastAddValue("notStrictEqual".ToKeyString(), new JSFunction(context, JSAssert.NotStrictEqual, "notStrictEqual" ,"function notStrictEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Equal as equal
@class.FastAddValue("equal".ToKeyString(), new JSFunction(context, JSAssert.Equal, "equal" ,"function equal() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting DoubleEqual as doubleEqual
@class.FastAddValue("doubleEqual".ToKeyString(), new JSFunction(context, JSAssert.DoubleEqual, "doubleEqual" ,"function doubleEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting NotEqual as notEqual
@class.FastAddValue("notEqual".ToKeyString(), new JSFunction(context, JSAssert.NotEqual, "notEqual" ,"function notEqual() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Throws as throws
@class.FastAddValue("throws".ToKeyString(), new JSFunction(context, JSAssert.Throws, "throws" ,"function throws() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Fail as fail
@class.FastAddValue("fail".ToKeyString(), new JSFunction(context, JSAssert.Fail, "fail" ,"function fail() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Match as match
@class.FastAddValue("match".ToKeyString(), new JSFunction(context, JSAssert.Match, "match" ,"function match() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
