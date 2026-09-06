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
                            context[KeyString.Promise] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Then as then
prototype.FastAddValue(KeyString.then, new JSFunction(context, (in Arguments a) =>
                    a.This is JSPromise @this
                        ? @this.Then(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSPromise")
                        , "then"
                        ,"function then() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Catch as catch
prototype.FastAddValue(KeyString.@catch, new JSFunction(context, (in Arguments a) => {
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
prototype.FastAddValue(KeyString.@finally, new JSFunction(context, (in Arguments a) => {
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
@class.FastAddValue(KeyString.resolve, new JSFunction(context, JSPromise.Resolve, "resolve" ,"function resolve() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Reject as reject
@class.FastAddValue(KeyString.reject, new JSFunction(context, JSPromise.Reject, "reject" ,"function reject() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting All as all
@class.FastAddValue(KeyString.all, new JSFunction(context, JSPromise.All, "all" ,"function all() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
context.Promise_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
