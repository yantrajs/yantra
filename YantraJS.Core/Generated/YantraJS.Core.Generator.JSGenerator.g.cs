using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Generator { 
partial class JSGenerator {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSGenerator(in a)
                            , "Generator"
                            , "function Generator() { [native code] }"
                            );
                        if (register) {
                            context["Generator".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting ToString as toString
prototype.FastAddValue("toString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSGenerator @this
                        ? @this.ToString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSGenerator")
                        , "toString"
                        ,"function toString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Next as next
prototype.FastAddValue("next".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSGenerator @this
                        ? @this.Next(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSGenerator")
                        , "next"
                        ,"function next() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Return as return
prototype.FastAddValue("return".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSGenerator @this
                        ? @this.Return(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSGenerator")
                        , "return"
                        ,"function return() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Throw as throw
prototype.FastAddValue("throw".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSGenerator @this
                        ? @this.Throw(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSGenerator")
                        , "throw"
                        ,"function throw() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
