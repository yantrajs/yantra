using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace Yantra.Core.Events { 
partial class EventTarget {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new EventTarget(in a)
                            , "EventTarget"
                            , "function EventTarget() { [native code] }"
                            );
                        if (register) {
                            context["EventTarget".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting DispatchEvent as dispatchEvent
prototype.FastAddValue("dispatchEvent".ToKeyString(), new JSFunction(context, (in Arguments a) => {
			if(!(a.This is EventTarget @this))
							throw JSContext.Current.NewTypeError("Failed to convert this to EventTarget");
			var pe = JSValueToClrConverter.GetAsOrThrow<Yantra.Core.Events.Event>(a[0], "e");
			var @return = @this.DispatchEvent(pe);
			return @return;
			}
			,
                "DispatchEvent"
                ,"function dispatchEvent() { [native] }", createPrototype: false
            ), JSPropertyAttributes.ConfigurableValue);
// Exporting AddEventListener as addEventListener
prototype.FastAddValue("addEventListener".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is EventTarget @this
                        ? @this.AddEventListener(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to EventTarget")
                        , "addEventListener"
                        ,"function addEventListener() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting RemoveEventListener as removeEventListener
prototype.FastAddValue("removeEventListener".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is EventTarget @this
                        ? @this.RemoveEventListener(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to EventTarget")
                        , "removeEventListener"
                        ,"function removeEventListener() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
