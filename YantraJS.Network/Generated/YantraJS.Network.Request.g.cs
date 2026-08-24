using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class Request {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new Request(in a)
                            , "Request"
                            , "function Request() { [native code] }"
                            );
                        if (register) {
                            context["Request".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Url as url
prototype.FastAddProperty(
                "url".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Url)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get url"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Method as method
prototype.FastAddProperty(
                "method".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Method)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get method"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Headers as headers
prototype.FastAddProperty(
                "headers".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Headers)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get headers"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Body as body
prototype.FastAddProperty(
                "body".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Body)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get body"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Body = JSValueToClrConverter.GetAsOrThrow<JSValue?>(a[0], "body");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set body"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Mode as mode
prototype.FastAddProperty(
                "mode".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Mode)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get mode"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Mode = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "mode");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set mode"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Credentials as credentials
prototype.FastAddProperty(
                "credentials".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Credentials)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get credentials"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Credentials = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "credentials");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set credentials"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Cache as cache
prototype.FastAddProperty(
                "cache".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Cache)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get cache"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Cache = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "cache");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set cache"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Redirect as redirect
prototype.FastAddProperty(
                "redirect".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Redirect)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get redirect"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Redirect = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "redirect");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set redirect"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Referrer as referrer
prototype.FastAddProperty(
                "referrer".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Referrer)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get referrer"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Referrer = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "referrer");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set referrer"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting ReferrerPolicy as referrerPolicy
prototype.FastAddProperty(
                "referrerPolicy".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.ReferrerPolicy)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get referrerPolicy"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.ReferrerPolicy = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "referrerPolicy");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set referrerPolicy"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Integrity as integrity
prototype.FastAddProperty(
                "integrity".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Integrity)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get integrity"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Integrity = JSValueToClrConverter.GetAsOrThrow<string?>(a[0], "integrity");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set integrity"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting KeepAlive as keepAlive
prototype.FastAddProperty(
                "keepAlive".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.KeepAlive)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get keepAlive"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.KeepAlive = JSValueToClrConverter.ToBoolean(a[0], "keepAlive");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set keepAlive"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Signal as signal
prototype.FastAddProperty(
                "signal".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Request @this
                        ? ClrProxy.Marshal(@this.Signal)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Request") ,
                "get signal"),
                new JSFunction(context, (in Arguments a) => {
                    if(a.This is Request @this) {
                         @this.Signal = JSValueToClrConverter.GetAsOrThrow<YantraJS.Network.AbortSignal?>(a[0], "signal");
                    }
                        else throw JSContext.Current.NewTypeError("Failed to convert this to Request");
                    return JSUndefined.Value;
                },
                "set signal"),
                JSPropertyAttributes.ConfigurableProperty);
return @class;
}
}
}
