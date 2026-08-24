using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class KeyValueStore {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new KeyValueStore(in a)
                            , "KeyValueStore"
                            , "function KeyValueStore() { [native code] }"
                            );
                        if (register) {
                            context["KeyValueStore".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Append as append
prototype.FastAddValue("append".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
			var pname = JSValueToClrConverter.ToString(a[0], "name");
			var pvalue = JSValueToClrConverter.ToString(a[1], "value");
			@this.Append(pname, pvalue);
			return JSUndefined.Value;
			}
			,
                "Append"
                ,"function append() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Delete as delete
prototype.FastAddValue("delete".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
			var pname = JSValueToClrConverter.ToString(a[0], "name");
			@this.Delete(pname);
			return JSUndefined.Value;
			}
			,
                "Delete"
                ,"function delete() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Entries as entries
prototype.FastAddValue("entries".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
			var @return = @this.Entries();
			return ClrProxy.Marshal(@return);
			}
			,
                "Entries"
                ,"function entries() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting ForEach as forEach
prototype.FastAddValue("forEach".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is KeyValueStore @this
                        ? @this.ForEach(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore")
                        , "forEach"
                        ,"function forEach() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Get as get
prototype.FastAddValue("get".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
			var pname = JSValueToClrConverter.ToString(a[0], "name");
			var @return = @this.Get(pname);
			return @return;
			}
			,
                "Get"
                ,"function get() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Has as has
prototype.FastAddValue("has".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
			var pname = JSValueToClrConverter.ToString(a[0], "name");
			var @return = @this.Has(pname);
			return @return;
			}
			,
                "Has"
                ,"function has() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Keys as keys
prototype.FastAddValue("keys".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
			var @return = @this.Keys();
			return ClrProxy.Marshal(@return);
			}
			,
                "Keys"
                ,"function keys() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Set as set
prototype.FastAddValue("set".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
			var pname = JSValueToClrConverter.ToString(a[0], "name");
			var pvalue = JSValueToClrConverter.ToString(a[1], "value");
			@this.Set(pname, pvalue);
			return JSUndefined.Value;
			}
			,
                "Set"
                ,"function set() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Values as values
prototype.FastAddValue("values".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is KeyValueStore @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to KeyValueStore");
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
