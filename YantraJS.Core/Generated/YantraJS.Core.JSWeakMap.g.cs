using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSWeakMap {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSWeakMap(in a)
                            , "WeakMap"
                            , "function WeakMap() { [native code] }"
                            );
                        if (register) {
                            context["WeakMap".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Set as set
prototype.FastAddValue("set".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSWeakMap @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSWeakMap");
			var pkey = a[0] is JSObject objkey ? objkey : throw new JSException("key is not an object");
			var pvalue = a[1];
			var @return = @this.Set(pkey, pvalue);
			return @return;
			}
			,
                "Set"
                ,"function set() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Delete as delete
prototype.FastAddValue("delete".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSWeakMap @this
                        ? @this.Delete(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSWeakMap")
                        , "delete"
                        ,"function delete() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Has as has
prototype.FastAddValue("has".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSWeakMap @this
                        ? @this.Has(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSWeakMap")
                        , "has"
                        ,"function has() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Get as get
prototype.FastAddValue("get".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSWeakMap @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSWeakMap");
			var pkey = a[0] is JSObject objkey ? objkey : throw new JSException("key is not an object");
			var @return = @this.Get(pkey);
			return @return;
			}
			,
                "Get"
                ,"function get() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
