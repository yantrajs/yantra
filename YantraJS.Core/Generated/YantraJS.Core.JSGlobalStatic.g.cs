using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSGlobalStatic {
public static new JSObject CreateClass(JSContext context, bool register = true) {

                    var @class = context;
// Exporting Infinity as Infinity
@class.FastAddProperty(
                "Infinity".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSGlobalStatic.Infinity),
                "get Infinity"),
                new JSFunction(context, (in Arguments a) => {
                    JSGlobalStatic.Infinity = JSValueToClrConverter.ToJSNumber(a[0], "Infinity");
                    return JSUndefined.Value;
                },
                "set Infinity"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting NaN as NaN
@class.FastAddProperty(
                "NaN".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSGlobalStatic.NaN),
                "get NaN"),
                new JSFunction(context, (in Arguments a) => {
                    JSGlobalStatic.NaN = JSValueToClrConverter.ToJSNumber(a[0], "NaN");
                    return JSUndefined.Value;
                },
                "set NaN"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Intl as Intl
@class.FastAddProperty(
                "Intl".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    JSGlobalStatic.Intl,
                "get Intl"),
                new JSFunction(context, (in Arguments a) => {
                    JSGlobalStatic.Intl = a[0];
                    return JSUndefined.Value;
                },
                "set Intl"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting DecodeURI as decodeURI
@class.FastAddValue("decodeURI".ToKeyString(), new JSFunction(context, JSGlobalStatic.DecodeURI, "decodeURI" ,"function decodeURI() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting DecodeURIComponent as decodeURIComponent
@class.FastAddValue("decodeURIComponent".ToKeyString(), new JSFunction(context, JSGlobalStatic.DecodeURIComponent, "decodeURIComponent" ,"function decodeURIComponent() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Eval as eval
@class.FastAddValue("eval".ToKeyString(), new JSFunction(context, JSGlobalStatic.Eval, "eval" ,"function eval() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting EncodeURI as encodeURI
@class.FastAddValue("encodeURI".ToKeyString(), new JSFunction(context, JSGlobalStatic.EncodeURI, "encodeURI" ,"function encodeURI() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting EncodeURIComponent as encodeURIComponent
@class.FastAddValue("encodeURIComponent".ToKeyString(), new JSFunction(context, JSGlobalStatic.EncodeURIComponent, "encodeURIComponent" ,"function encodeURIComponent() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting IsFinite as isFinite
@class.FastAddValue("isFinite".ToKeyString(), new JSFunction(context, JSGlobalStatic.IsFinite, "isFinite" ,"function isFinite() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting IsNaN as isNaN
@class.FastAddValue("isNaN".ToKeyString(), new JSFunction(context, JSGlobalStatic.IsNaN, "isNaN" ,"function isNaN() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ParseFloat as parseFloat
@class.FastAddValue("parseFloat".ToKeyString(), new JSFunction(context, JSGlobalStatic.ParseFloat, "parseFloat" ,"function parseFloat() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ParseInt as parseInt
@class.FastAddValue("parseInt".ToKeyString(), new JSFunction(context, JSGlobalStatic.ParseInt, "parseInt" ,"function parseInt() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetImmediate as setImmediate
@class.FastAddValue("setImmediate".ToKeyString(), new JSFunction(context, JSGlobalStatic.SetImmediate, "setImmediate" ,"function setImmediate() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting SetInterval as setInterval
@class.FastAddValue("setInterval".ToKeyString(), new JSFunction(context, JSGlobalStatic.SetInterval, "setInterval" ,"function setInterval() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting ClearInterval as clearInterval
@class.FastAddValue("clearInterval".ToKeyString(), new JSFunction(context, JSGlobalStatic.ClearInterval, "clearInterval" ,"function clearInterval() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting SetTimeout as setTimeout
@class.FastAddValue("setTimeout".ToKeyString(), new JSFunction(context, JSGlobalStatic.SetTimeout, "setTimeout" ,"function setTimeout() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting ClearTimeout as clearTimeout
@class.FastAddValue("clearTimeout".ToKeyString(), new JSFunction(context, JSGlobalStatic.ClearTimeout, "clearTimeout" ,"function clearTimeout() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
