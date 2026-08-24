using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSMap {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSMap(in a)
                            , "Map"
                            , "function Map() { [native code] }"
                            );
                        if (register) {
                            context["Map".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Size as size
prototype.FastAddProperty(
                "size".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSMap @this
                        ? ClrProxy.Marshal(@this.Size)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSMap") ,
                "get size"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Set as set
prototype.FastAddValue("set".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSMap @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSMap");
			var pkey = a[0];
			var pvalue = a[1];
			var @return = @this.Set(pkey, pvalue);
			return @return;
			}
			,
                "Set"
                ,"function set() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Set as clear
prototype.FastAddValue("clear".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSMap @this
                        ? @this.Set(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSMap")
                        , "clear"
                        ,"function clear() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Delete as delete
prototype.FastAddValue("delete".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSMap @this
                        ? @this.Delete(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSMap")
                        , "delete"
                        ,"function delete() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetEntries as entries
prototype.FastAddValue("entries".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSMap @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSMap");
			var @return = @this.GetEntries();
			return ClrProxy.Marshal(@return);
			}
			,
                "GetEntries"
                ,"function entries() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting ForEach as forEach
prototype.FastAddValue("forEach".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSMap @this
                        ? @this.ForEach(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSMap")
                        , "forEach"
                        ,"function forEach() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Has as has
prototype.FastAddValue("has".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSMap @this
                        ? @this.Has(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSMap")
                        , "has"
                        ,"function has() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Get as get
prototype.FastAddValue("get".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSMap @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSMap");
			var pkey = a[0];
			var @return = @this.Get(pkey);
			return @return;
			}
			,
                "Get"
                ,"function get() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Keys as keys
prototype.FastAddValue("keys".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSMap @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSMap");
			var @return = @this.Keys();
			return ClrProxy.Marshal(@return);
			}
			,
                "Keys"
                ,"function keys() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Values as values
prototype.FastAddValue("values".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSMap @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSMap");
			var @return = @this.Values();
			return ClrProxy.Marshal(@return);
			}
			,
                "Values"
                ,"function values() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
