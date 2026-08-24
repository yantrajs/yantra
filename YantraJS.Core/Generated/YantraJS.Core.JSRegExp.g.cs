using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSRegExp {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSRegExp(in a)
                            , "RegExp"
                            , "function RegExp() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.RegExp] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting pattern as source
prototype.FastAddProperty(
                KeyString.source,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? ClrProxy.Marshal(@this.pattern)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp") ,
                "get source"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting flags as flags
prototype.FastAddProperty(
                KeyString.flags,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? ClrProxy.Marshal(@this.flags)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp") ,
                "get flags"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting globalSearch as global
prototype.FastAddProperty(
                KeyString.global,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? ClrProxy.Marshal(@this.globalSearch)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp") ,
                "get global"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting multiline as multiline
prototype.FastAddProperty(
                KeyString.multiline,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? ClrProxy.Marshal(@this.multiline)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp") ,
                "get multiline"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting ignoreCase as ignoreCase
prototype.FastAddProperty(
                KeyString.ignoreCase,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? ClrProxy.Marshal(@this.ignoreCase)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp") ,
                "get ignoreCase"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting lastIndex as lastIndex
prototype.FastAddProperty(
                KeyString.lastIndex,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? ClrProxy.Marshal(@this.lastIndex)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp") ,
                "get lastIndex"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSRegExp @this) {
                         @this.lastIndex = JSValueToClrConverter.ToInt(a[0], "lastIndex");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp");
                    return JSUndefined.Value;
                },
                "set lastIndex"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting LastIndex as lastIndex
prototype.FastAddProperty(
                KeyString.lastIndex,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? ClrProxy.Marshal(@this.LastIndex)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp") ,
                "get lastIndex"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSRegExp @this) {
                         @this.LastIndex = JSValueToClrConverter.ToInt(a[0], "lastIndex");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp");
                    return JSUndefined.Value;
                },
                "set lastIndex"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Test as test
prototype.FastAddValue(KeyString.test, new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? @this.Test(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp")
                        , "test"
                        ,"function test() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Exec as exec
prototype.FastAddValue(KeyString.exec, new JSFunction(context, (in Arguments a) =>
                    a.This is JSRegExp @this
                        ? @this.Exec(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSRegExp")
                        , "exec"
                        ,"function exec() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting ToString as toString
prototype.FastAddValue(KeyString.toString, new JSFunction(context, JSRegExp.ToString, "toString" ,"function toString() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
// Exporting ToLocaleString as toLocaleString
prototype.FastAddValue(KeyString.toLocaleString, new JSFunction(context, JSRegExp.ToLocaleString, "toLocaleString" ,"function toLocaleString() { [native] }", createPrototype: false, length: 1), JSPropertyAttributes.ConfigurableValue);
context.RegExp_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
