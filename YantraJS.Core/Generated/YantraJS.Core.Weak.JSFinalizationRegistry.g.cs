using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Weak { 
partial class JSFinalizationRegistry {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSFinalizationRegistry(in a)
                            , "FinalizationRegistry"
                            , "function FinalizationRegistry() { [native code] }"
                            );
                        if (register) {
                            context["FinalizationRegistry".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Unregister as unregister
prototype.FastAddValue("unregister".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSFinalizationRegistry @this
                        ? @this.Unregister(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSFinalizationRegistry")
                        , "unregister"
                        ,"function unregister() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Register as register
prototype.FastAddValue("register".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSFinalizationRegistry @this
                        ? @this.Register(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSFinalizationRegistry")
                        , "register"
                        ,"function register() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
