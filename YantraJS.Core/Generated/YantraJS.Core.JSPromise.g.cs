using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSPromise {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => new JSPromise(in a)
                            , "Promise"
                            , "function Promise() { [native code] }"
                            );
                        if (register) {
                            context["Promise".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Then as then
prototype.FastAddValue("then".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSPromise @this
                        ? @this.Then(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSPromise")
                        , "then"
                        ,"function then() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Catch as catch
prototype.FastAddValue("catch".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSPromise @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSPromise");
			var pfx = a[0] is JSFunction objfx ? objfx : throw new JSException("fx is not a function");
			var @return = @this.Catch(pfx);
			return @return;
			}
			,
                "Catch"
                ,"function catch() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Finally as finally
prototype.FastAddValue("finally".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSPromise @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSPromise");
			var pfx = a[0] is JSFunction objfx ? objfx : throw new JSException("fx is not a function");
			var @return = @this.Finally(pfx);
			return @return;
			}
			,
                "Finally"
                ,"function finally() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Resolve as resolve
@class.FastAddValue("resolve".ToKeyString(), new JSFunction(context, JSPromise.Resolve, "resolve" ,"function resolve() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Reject as resolve
@class.FastAddValue("resolve".ToKeyString(), new JSFunction(context, JSPromise.Reject, "resolve" ,"function resolve() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting All as resolve
@class.FastAddValue("resolve".ToKeyString(), new JSFunction(context, JSPromise.All, "resolve" ,"function resolve() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
