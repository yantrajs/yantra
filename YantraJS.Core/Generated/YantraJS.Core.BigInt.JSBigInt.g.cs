using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.BigInt { 
partial class JSBigInt {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSBigInt.Constructor(in a)
                            , "BigInt"
                            , "function BigInt() { [native code] }"
                            );
                        if (register) {
                            context["BigInt".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting JSToString as toString
prototype.FastAddValue("toString".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSBigInt @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSBigInt");
			var @return = @this.JSToString();
			return @return;
			}
			,
                "JSToString"
                ,"function toString() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue("toLocaleString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSBigInt @this
                        ? @this.ToLocaleString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSBigInt")
                        , "toLocaleString"
                        ,"function toLocaleString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ValueOf as valueOf
prototype.FastAddValue("valueOf".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is JSBigInt @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to JSBigInt");
			var @return = @this.ValueOf();
			return @return;
			}
			,
                "ValueOf"
                ,"function valueOf() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting AsIntN as asIntN
@class.FastAddValue("asIntN".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			var pbits = JSValueToClrConverter.ToLong(a[0], "bits");
			var pbigint = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.BigInt.JSBigInt>(a[1], "bigint");
			var @return = JSBigInt.AsIntN(pbits, pbigint);
			return @return;
			}
			,
                "AsIntN"
                ,"function asIntN() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting AsUintN as asUintN
@class.FastAddValue("asUintN".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			var pbits = JSValueToClrConverter.ToLong(a[0], "bits");
			var pbigint = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.BigInt.JSBigInt>(a[1], "bigint");
			var @return = JSBigInt.AsUintN(pbits, pbigint);
			return @return;
			}
			,
                "AsUintN"
                ,"function asUintN() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
