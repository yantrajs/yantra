using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSNumber {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSNumber.Constructor(in a)
                            , "Number"
                            , "function Number() { [native code] }"
                            , length:1);
                        if (register) {
                            context[KeyString.Number] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting NaN as NaN
@class.FastAddProperty(
                KeyString.NaN,
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSNumber.NaN),
                "get NaN"),
                new JSFunction(context, (in Arguments a) => {
                    JSNumber.NaN = JSValueToClrConverter.ToJSNumber(a[0], "NaN");
                    return JSUndefined.Value;
                },
                "set NaN"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting PositiveInfinity as POSITIVE_INFINITY
@class.FastAddProperty(
                KeyString.POSITIVE_INFINITY,
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSNumber.PositiveInfinity),
                "get POSITIVE_INFINITY"),
                new JSFunction(context, (in Arguments a) => {
                    JSNumber.PositiveInfinity = JSValueToClrConverter.ToJSNumber(a[0], "POSITIVE_INFINITY");
                    return JSUndefined.Value;
                },
                "set POSITIVE_INFINITY"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting NegativeInfinity as NEGATIVE_INFINITY
@class.FastAddProperty(
                KeyString.NEGATIVE_INFINITY,
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSNumber.NegativeInfinity),
                "get NEGATIVE_INFINITY"),
                new JSFunction(context, (in Arguments a) => {
                    JSNumber.NegativeInfinity = JSValueToClrConverter.ToJSNumber(a[0], "NEGATIVE_INFINITY");
                    return JSUndefined.Value;
                },
                "set NEGATIVE_INFINITY"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Epsilon as EPSILON
@class.FastAddValue(
                KeyString.EPSILON,
                ClrProxy.Marshal(JSNumber.Epsilon),
                JSPropertyAttributes.ReadonlyValue);
// Exporting MaxSafeInteger as MAX_SAFE_INTEGER
@class.FastAddValue(
                KeyString.MAX_SAFE_INTEGER,
                ClrProxy.Marshal(JSNumber.MaxSafeInteger),
                JSPropertyAttributes.ReadonlyValue);
// Exporting MaxValue as MAX_VALUE
@class.FastAddValue(
                KeyString.MAX_VALUE,
                ClrProxy.Marshal(JSNumber.MaxValue),
                JSPropertyAttributes.ReadonlyValue);
// Exporting MinSafeInteger as MIN_SAFE_INTEGER
@class.FastAddValue(
                KeyString.MIN_SAFE_INTEGER,
                ClrProxy.Marshal(JSNumber.MinSafeInteger),
                JSPropertyAttributes.ReadonlyValue);
// Exporting MinValue as MIN_VALUE
@class.FastAddValue(
                KeyString.MIN_VALUE,
                ClrProxy.Marshal(JSNumber.MinValue),
                JSPropertyAttributes.ReadonlyValue);
// Exporting Clz as clz
prototype.FastAddValue(KeyString.clz, new JSFunction(context, JSNumber.Clz, "clz" ,"function clz() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ValueOf as valueOf
prototype.FastAddValue(KeyString.valueOf, new JSFunction(context, JSNumber.ValueOf, "valueOf" ,"function valueOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToString as toString
prototype.FastAddValue(KeyString.toString, new JSFunction(context, JSNumber.ToString, "toString" ,"function toString() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToExponential as toExponential
prototype.FastAddValue(KeyString.toExponential, new JSFunction(context, JSNumber.ToExponential, "toExponential" ,"function toExponential() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToFixed as toFixed
prototype.FastAddValue(KeyString.toFixed, new JSFunction(context, JSNumber.ToFixed, "toFixed" ,"function toFixed() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToPrecision as toPrecision
prototype.FastAddValue(KeyString.toPrecision, new JSFunction(context, JSNumber.ToPrecision, "toPrecision" ,"function toPrecision() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue(KeyString.toLocaleString, new JSFunction(context, JSNumber.ToLocaleString, "toLocaleString" ,"function toLocaleString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsFinite as isFinite
@class.FastAddValue(KeyString.isFinite, new JSFunction(context, JSNumber.IsFinite, "isFinite" ,"function isFinite() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsInteger as isInteger
@class.FastAddValue(KeyString.isInteger, new JSFunction(context, JSNumber.IsInteger, "isInteger" ,"function isInteger() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsNaN as isNaN
@class.FastAddValue(KeyString.isNaN, new JSFunction(context, JSNumber.IsNaN, "isNaN" ,"function isNaN() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting IsSafeInteger as isSafeInteger
@class.FastAddValue(KeyString.isSafeInteger, new JSFunction(context, JSNumber.IsSafeInteger, "isSafeInteger" ,"function isSafeInteger() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ParseFloat as parseFloat
@class.FastAddValue(KeyString.parseFloat, new JSFunction(context, JSNumber.ParseFloat, "parseFloat" ,"function parseFloat() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ParseInt as parseInt
@class.FastAddValue(KeyString.parseInt, new JSFunction(context, JSNumber.ParseInt, "parseInt" ,"function parseInt() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
context.Number_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
