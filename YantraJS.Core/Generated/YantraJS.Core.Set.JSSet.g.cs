using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Set { 
partial class JSSet {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSSet(in a)
                            , "Set"
                            , "function Set() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.Set] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Size as size
prototype.FastAddProperty(
                KeyString.size,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSSet @this
                        ? ClrProxy.Marshal(@this.Size)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSSet") ,
                "get size"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Add as add
prototype.FastAddValue(KeyString.add, new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSSet @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSSet");
			var pkey = a[0];
			var @return = @this.Add(pkey);
			return @return;
			}
			,
                "Add"
                ,"function add() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Set as clear
prototype.FastAddValue(KeyString.clear, new JSFunction(context, (in Arguments a) =>
                    a.This is JSSet @this
                        ? @this.Set(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSSet")
                        , "clear"
                        ,"function clear() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Delete as delete
prototype.FastAddValue(KeyString.delete, new JSFunction(context, (in Arguments a) =>
                    a.This is JSSet @this
                        ? @this.Delete(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSSet")
                        , "delete"
                        ,"function delete() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting GetEntries as entries
prototype.FastAddValue(KeyString.entries, new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSSet @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSSet");
			var @return = @this.GetEntries();
			return ClrProxy.Marshal(@return);
			}
			,
                "GetEntries"
                ,"function entries() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting ForEach as forEach
prototype.FastAddValue(KeyString.forEach, new JSFunction(context, (in Arguments a) =>
                    a.This is JSSet @this
                        ? @this.ForEach(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSSet")
                        , "forEach"
                        ,"function forEach() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Has as has
prototype.FastAddValue(KeyString.has, new JSFunction(context, (in Arguments a) =>
                    a.This is JSSet @this
                        ? @this.Has(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSSet")
                        , "has"
                        ,"function has() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Keys as keys
prototype.FastAddValue(KeyString.keys, new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSSet @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSSet");
			var @return = @this.Keys();
			return ClrProxy.Marshal(@return);
			}
			,
                "Keys"
                ,"function keys() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Values as values
prototype.FastAddValue(KeyString.values, new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSSet @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSSet");
			var @return = @this.Values();
			return ClrProxy.Marshal(@return);
			}
			,
                "Values"
                ,"function values() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
context.Set_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
