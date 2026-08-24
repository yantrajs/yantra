using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Set { 
partial class JSWeakSet {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSWeakSet(in a)
                            , "WeakSet"
                            , "function WeakSet() { [native code] }"
                            );
                        if (register) {
                            context["WeakSet".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Add as add
prototype.FastAddValue("add".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSWeakSet @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSWeakSet");
			var pa = a[0] is JSObject obja ? obja : throw new JSException("a is not an object");
			var @return = @this.Add(pa);
			return @return;
			}
			,
                "Add"
                ,"function add() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Delete as delete
prototype.FastAddValue("delete".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSWeakSet @this
                        ? @this.Delete(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSWeakSet")
                        , "delete"
                        ,"function delete() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
