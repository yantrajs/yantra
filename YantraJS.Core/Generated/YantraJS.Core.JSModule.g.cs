using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSModule {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => new JSModule(in a)
                            , "Module"
                            , "function Module() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.Module] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Code as code
prototype.FastAddProperty(
                KeyString.code,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSModule @this
                        ? ClrProxy.Marshal(@this.Code)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSModule") ,
                "get code"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSModule @this) {
                         @this.Code = JSValueToClrConverter.ToString(a[0], "code");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSModule");
                    return JSUndefined.Value;
                },
                "set code"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Id as id
prototype.FastAddProperty(
                KeyString.id,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSModule @this
                        ? @this.Id
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSModule") ,
                "get id"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Exports as exports
prototype.FastAddProperty(
                KeyString.exports,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSModule @this
                        ? @this.Exports
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSModule") ,
                "get exports"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSModule @this) {
                         @this.Exports = a[0];
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSModule");
                    return JSUndefined.Value;
                },
                "set exports"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Require as require
prototype.FastAddProperty(
                KeyString.require,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSModule @this
                        ? @this.Require
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSModule") ,
                "get require"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSModule @this) {
                         @this.Require = a[0];
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSModule");
                    return JSUndefined.Value;
                },
                "set require"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Import as import
prototype.FastAddProperty(
                KeyString.import,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSModule @this
                        ? @this.Import
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSModule") ,
                "get import"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSModule @this) {
                         @this.Import = a[0];
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSModule");
                    return JSUndefined.Value;
                },
                "set import"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Compile as compile
prototype.FastAddProperty(
                KeyString.compile,
                new JSFunction(context, (in Arguments a) =>
                    a.This is JSModule @this
                        ? @this.Compile
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSModule") ,
                "get compile"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is JSModule @this) {
                         @this.Compile = a[0];
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to JSModule");
                    return JSUndefined.Value;
                },
                "set compile"),
                JSPropertyAttributes.ConfigurableProperty);
context.Module_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
