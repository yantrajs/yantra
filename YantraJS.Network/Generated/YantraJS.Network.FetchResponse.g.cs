using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class FetchResponse {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new FetchResponse(in a)
                            , "FetchResponse"
                            , "function FetchResponse() { [native code] }"
                            );
                        if (register) {
                            context["FetchResponse".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Ok as ok
prototype.FastAddProperty(
                "ok".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is FetchResponse @this
                        ? ClrProxy.Marshal(@this.Ok)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse") ,
                "get ok"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Status as status
prototype.FastAddProperty(
                "status".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is FetchResponse @this
                        ? ClrProxy.Marshal(@this.Status)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse") ,
                "get status"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Redirected as redirected
prototype.FastAddProperty(
                "redirected".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is FetchResponse @this
                        ? ClrProxy.Marshal(@this.Redirected)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse") ,
                "get redirected"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Type as type
prototype.FastAddProperty(
                "type".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is FetchResponse @this
                        ? ClrProxy.Marshal(@this.Type)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse") ,
                "get type"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Url as url
prototype.FastAddProperty(
                "url".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is FetchResponse @this
                        ? ClrProxy.Marshal(@this.Url)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse") ,
                "get url"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Headers as headers
prototype.FastAddProperty(
                "headers".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is FetchResponse @this
                        ? ClrProxy.Marshal(@this.Headers)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse") ,
                "get headers"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Json as json
prototype.FastAddValue("json".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is FetchResponse @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse");
			var @return = @this.Json();
			return @return;
			}
			,
                "Json"
                ,"function json() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting Text as text
prototype.FastAddValue("text".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is FetchResponse @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to FetchResponse");
			var @return = @this.Text();
			return @return;
			}
			,
                "Text"
                ,"function text() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
