using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSDate {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => new JSDate(in a)
                            , "Date"
                            , "function Date() { [native code] }"
                            , length:7);
                        if (register) {
                            context["Date".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting GetDate as getDate
prototype.FastAddValue("getDate".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetDate(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getDate"
                        ,"function getDate() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting SetDate as setDate
prototype.FastAddValue("setDate".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetDate(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setDate"
                        ,"function setDate() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting GetDay as getDay
prototype.FastAddValue("getDay".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetDay(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getDay"
                        ,"function getDay() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetFullYear as getFullYear
prototype.FastAddValue("getFullYear".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetFullYear(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getFullYear"
                        ,"function getFullYear() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetHours as getHours
prototype.FastAddValue("getHours".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetHours(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getHours"
                        ,"function getHours() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetMilliSeconds as getMilliseconds
prototype.FastAddValue("getMilliseconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetMilliSeconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getMilliseconds"
                        ,"function getMilliseconds() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetMinutes as getMinutes
prototype.FastAddValue("getMinutes".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetMinutes(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getMinutes"
                        ,"function getMinutes() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetMonth as getMonth
prototype.FastAddValue("getMonth".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetMonth(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getMonth"
                        ,"function getMonth() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetSeconds as getSeconds
prototype.FastAddValue("getSeconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetSeconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getSeconds"
                        ,"function getSeconds() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetTime as getTime
prototype.FastAddValue("getTime".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetTime(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getTime"
                        ,"function getTime() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetTimezoneOffset as getTimezoneOffset
prototype.FastAddValue("getTimezoneOffset".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetTimezoneOffset(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getTimezoneOffset"
                        ,"function getTimezoneOffset() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCDate as getUTCDate
prototype.FastAddValue("getUTCDate".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCDate(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCDate"
                        ,"function getUTCDate() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCDay as getUTCDay
prototype.FastAddValue("getUTCDay".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCDay(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCDay"
                        ,"function getUTCDay() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCFullYear as getUTCFullYear
prototype.FastAddValue("getUTCFullYear".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCFullYear(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCFullYear"
                        ,"function getUTCFullYear() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCHours as getUTCHours
prototype.FastAddValue("getUTCHours".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCHours(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCHours"
                        ,"function getUTCHours() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCMilliseconds as getUTCMilliseconds
prototype.FastAddValue("getUTCMilliseconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCMilliseconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCMilliseconds"
                        ,"function getUTCMilliseconds() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCMinutes as getUTCMinutes
prototype.FastAddValue("getUTCMinutes".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCMinutes(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCMinutes"
                        ,"function getUTCMinutes() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCMonth as getUTCMonth
prototype.FastAddValue("getUTCMonth".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCMonth(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCMonth"
                        ,"function getUTCMonth() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting GetUTCSeconds as getUTCSeconds
prototype.FastAddValue("getUTCSeconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.GetUTCSeconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "getUTCSeconds"
                        ,"function getUTCSeconds() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting SetFullYear as setFullYear
prototype.FastAddValue("setFullYear".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetFullYear(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setFullYear"
                        ,"function setFullYear() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetHours as setHours
prototype.FastAddValue("setHours".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetHours(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setHours"
                        ,"function setHours() { [native] }", createPrototype: false, length: 4), JSPropertyAttributes.ConfigurableValue);
// Exporting SetMilliseconds as setMilliseconds
prototype.FastAddValue("setMilliseconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetMilliseconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setMilliseconds"
                        ,"function setMilliseconds() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting SetMinutes as setMinutes
prototype.FastAddValue("setMinutes".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetMinutes(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setMinutes"
                        ,"function setMinutes() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetMonth as setMonth
prototype.FastAddValue("setMonth".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetMonth(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setMonth"
                        ,"function setMonth() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetSeconds as setSeconds
prototype.FastAddValue("setSeconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetSeconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setSeconds"
                        ,"function setSeconds() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetTime as setTime
prototype.FastAddValue("setTime".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetTime(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setTime"
                        ,"function setTime() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting setUTCDate as setUTCDate
prototype.FastAddValue("setUTCDate".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.setUTCDate(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setUTCDate"
                        ,"function setUTCDate() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting setUTCFullYear as setUTCFullYear
prototype.FastAddValue("setUTCFullYear".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.setUTCFullYear(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setUTCFullYear"
                        ,"function setUTCFullYear() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUTCHours as setUTCHours
prototype.FastAddValue("setUTCHours".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetUTCHours(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setUTCHours"
                        ,"function setUTCHours() { [native] }", createPrototype: false, length: 4), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUTCMilliseconds as setUTCMilliseconds
prototype.FastAddValue("setUTCMilliseconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetUTCMilliseconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setUTCMilliseconds"
                        ,"function setUTCMilliseconds() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUTCMinutes as setUTCMinutes
prototype.FastAddValue("setUTCMinutes".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetUTCMinutes(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setUTCMinutes"
                        ,"function setUTCMinutes() { [native] }", createPrototype: false, length: 3), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUTCMonth as setUTCMonth
prototype.FastAddValue("setUTCMonth".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetUTCMonth(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setUTCMonth"
                        ,"function setUTCMonth() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting SetUTCSeconds as setUTCSeconds
prototype.FastAddValue("setUTCSeconds".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.SetUTCSeconds(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "setUTCSeconds"
                        ,"function setUTCSeconds() { [native] }", createPrototype: false, length: 2), JSPropertyAttributes.ConfigurableValue);
// Exporting ToDateString as toDateString
prototype.FastAddValue("toDateString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToDateString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toDateString"
                        ,"function toDateString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToISOString as toISOString
prototype.FastAddValue("toISOString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToISOString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toISOString"
                        ,"function toISOString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToJSON as toJSON
prototype.FastAddValue("toJSON".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToJSON(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toJSON"
                        ,"function toJSON() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleDateString as toLocaleDateString
prototype.FastAddValue("toLocaleDateString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToLocaleDateString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toLocaleDateString"
                        ,"function toLocaleDateString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue("toLocaleString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToLocaleString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toLocaleString"
                        ,"function toLocaleString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleTimeString as toLocaleTimeString
prototype.FastAddValue("toLocaleTimeString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToLocaleTimeString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toLocaleTimeString"
                        ,"function toLocaleTimeString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToString as toString
prototype.FastAddValue("toString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toString"
                        ,"function toString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToTimeString as toTimeString
prototype.FastAddValue("toTimeString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToTimeString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toTimeString"
                        ,"function toTimeString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ToUTCString as toUTCString
prototype.FastAddValue("toUTCString".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ToUTCString(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "toUTCString"
                        ,"function toUTCString() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting ValueOf as valueOf
prototype.FastAddValue("valueOf".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSDate @this
                        ? @this.ValueOf(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSDate")
                        , "valueOf"
                        ,"function valueOf() { [native] }", createPrototype: false, length: 0), JSPropertyAttributes.ConfigurableValue);
// Exporting UTC as UTC
@class.FastAddValue("UTC".ToKeyString(), new JSFunction(context, JSDate.UTC, "UTC" ,"function UTC() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Now as now
@class.FastAddValue("now".ToKeyString(), new JSFunction(context, JSDate.Now, "now" ,"function now() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Parse as parse
@class.FastAddValue("parse".ToKeyString(), new JSFunction(context, JSDate.Parse, "parse" ,"function parse() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
