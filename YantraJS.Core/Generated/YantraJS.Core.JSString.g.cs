using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSString {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSString.Constructor(in a)
                            , "String"
                            , "function String() { [native code] }"
                            , length:1);
                        if (register) {
                            context[KeyString.String] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Length as length
prototype.FastAddProperty(
                KeyString.length,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSString @this
                        ? ClrProxy.Marshal(@this.Length)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSString") ,
                "get length"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSString @this) {
                         @this.Length = JSValueToClrConverter.ToInt(a[0], "length");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSString");
                    return JSUndefined.Value;
                },
                "set length"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting CharAt as charAt
prototype.FastAddValue(KeyString.charAt, new JSFunction(context, JSString.CharAt, "charAt" ,"function charAt() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Substring as substring
prototype.FastAddValue(KeyString.substring, new JSFunction(context, JSString.Substring, "substring" ,"function substring() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Substr as substr
prototype.FastAddValue(KeyString.substr, new JSFunction(context, JSString.Substr, "substr" ,"function substr() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToString as toString
prototype.FastAddValue(KeyString.toString, new JSFunction(context, JSString.ToString, "toString" ,"function toString() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting CharCodeAt as charCodeAt
prototype.FastAddValue(KeyString.charCodeAt, new JSFunction(context, JSString.CharCodeAt, "charCodeAt" ,"function charCodeAt() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting CodePointAt as codePointAt
prototype.FastAddValue(KeyString.codePointAt, new JSFunction(context, JSString.CodePointAt, "codePointAt" ,"function codePointAt() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Concat as concat
prototype.FastAddValue(KeyString.concat, new JSFunction(context, JSString.Concat, "concat" ,"function concat() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Contains as contains
prototype.FastAddValue(KeyString.contains, new JSFunction(context, JSString.Contains, "contains" ,"function contains() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting EndsWith as endsWith
prototype.FastAddValue(KeyString.endsWith, new JSFunction(context, JSString.EndsWith, "endsWith" ,"function endsWith() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting StartsWith as startsWith
prototype.FastAddValue(KeyString.startsWith, new JSFunction(context, JSString.StartsWith, "startsWith" ,"function startsWith() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Includes as includes
prototype.FastAddValue(KeyString.includes, new JSFunction(context, JSString.Includes, "includes" ,"function includes() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting IndexOf as indexOf
prototype.FastAddValue(KeyString.indexOf, new JSFunction(context, JSString.IndexOf, "indexOf" ,"function indexOf() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting LastIndexOF as lastIndexOf
prototype.FastAddValue(KeyString.lastIndexOf, new JSFunction(context, JSString.LastIndexOF, "lastIndexOf" ,"function lastIndexOf() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting LocaleCompare as localeCompare
prototype.FastAddValue(KeyString.localeCompare, new JSFunction(context, JSString.LocaleCompare, "localeCompare" ,"function localeCompare() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Match as match
prototype.FastAddValue(KeyString.match, new JSFunction(context, JSString.Match, "match" ,"function match() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Normalize as normalize
prototype.FastAddValue(KeyString.normalize, new JSFunction(context, JSString.Normalize, "normalize" ,"function normalize() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting PadEnd as padEnd
prototype.FastAddValue(KeyString.padEnd, new JSFunction(context, JSString.PadEnd, "padEnd" ,"function padEnd() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting PadStart as padStart
prototype.FastAddValue(KeyString.padStart, new JSFunction(context, JSString.PadStart, "padStart" ,"function padStart() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Repeat as repeat
prototype.FastAddValue(KeyString.repeat, new JSFunction(context, JSString.Repeat, "repeat" ,"function repeat() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Replace as replace
prototype.FastAddValue(KeyString.replace, new JSFunction(context, JSString.Replace, "replace" ,"function replace() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting ReplaceAll as replaceAll
prototype.FastAddValue(KeyString.replaceAll, new JSFunction(context, JSString.ReplaceAll, "replaceAll" ,"function replaceAll() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Search as search
prototype.FastAddValue(KeyString.search, new JSFunction(context, JSString.Search, "search" ,"function search() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Slice as slice
prototype.FastAddValue(KeyString.slice, new JSFunction(context, JSString.Slice, "slice" ,"function slice() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting Split as split
prototype.FastAddValue(KeyString.split, new JSFunction(context, JSString.Split, "split" ,"function split() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleLowerCase as toLocaleLowerCase
prototype.FastAddValue(KeyString.toLocaleLowerCase, new JSFunction(context, JSString.ToLocaleLowerCase, "toLocaleLowerCase" ,"function toLocaleLowerCase() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleUpperCase as toLocaleUpperCase
prototype.FastAddValue(KeyString.toLocaleUpperCase, new JSFunction(context, JSString.ToLocaleUpperCase, "toLocaleUpperCase" ,"function toLocaleUpperCase() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLowerCase as toLowerCase
prototype.FastAddValue(KeyString.toLowerCase, new JSFunction(context, JSString.ToLowerCase, "toLowerCase" ,"function toLowerCase() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToUpperCase as toUpperCase
prototype.FastAddValue(KeyString.toUpperCase, new JSFunction(context, JSString.ToUpperCase, "toUpperCase" ,"function toUpperCase() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Trim as trim
prototype.FastAddValue(KeyString.trim, new JSFunction(context, JSString.Trim, "trim" ,"function trim() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting TrimEnd as trimEnd
prototype.FastAddValue(KeyString.trimEnd, new JSFunction(context, JSString.TrimEnd, "trimEnd" ,"function trimEnd() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting TrimStart as trimStart
prototype.FastAddValue(KeyString.trimStart, new JSFunction(context, JSString.TrimStart, "trimStart" ,"function trimStart() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ValueOf as valueOf
prototype.FastAddValue(KeyString.valueOf, new JSFunction(context, JSString.ValueOf, "valueOf" ,"function valueOf() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting FromCharCode as fromCharCode
@class.FastAddValue(KeyString.fromCharCode, new JSFunction(context, JSString.FromCharCode, "fromCharCode" ,"function fromCharCode() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting FromCodePoint as fromCodePoint
@class.FastAddValue(KeyString.fromCodePoint, new JSFunction(context, JSString.FromCodePoint, "fromCodePoint" ,"function fromCodePoint() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting Raw as raw
@class.FastAddValue(KeyString.raw, new JSFunction(context, JSString.Raw, "raw" ,"function raw() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
context.String_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
