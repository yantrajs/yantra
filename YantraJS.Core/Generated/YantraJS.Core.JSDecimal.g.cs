using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSDecimal {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSDecimal.Constructor(in a)
                            , "Decimal"
                            , "function Decimal() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.Decimal] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting JSToString as toString
prototype.FastAddValue(KeyString.toString, new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSDecimal @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSDecimal");
			var @return = @this.JSToString();
			return @return;
			}
			,
                "JSToString"
                ,"function toString() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting JSToFixed as toFixed
prototype.FastAddValue(KeyString.toFixed, new JSFunction(context, (in Arguments a) =>
                    a.This is JSDecimal @this
                        ? @this.JSToFixed(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDecimal")
                        , "toFixed"
                        ,"function toFixed() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue(KeyString.toLocaleString, new JSFunction(context, (in Arguments a) =>
                    a.This is JSDecimal @this
                        ? @this.ToLocaleString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDecimal")
                        , "toLocaleString"
                        ,"function toLocaleString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ValueOf as valueOf
prototype.FastAddValue(KeyString.valueOf, new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSDecimal @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSDecimal");
			var @return = @this.ValueOf();
			return @return;
			}
			,
                "ValueOf"
                ,"function valueOf() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
context.Decimal_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
