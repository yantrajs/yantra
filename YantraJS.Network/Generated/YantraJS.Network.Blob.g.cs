using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class Blob {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new Blob(in a)
                            , "Blob"
                            , "function Blob() { [native code] }"
                            );
                        if (register) {
                            context["Blob".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting None as None
@class.FastAddValue(
                "None".ToKeyString(),
                ClrProxy.Marshal(Blob.None),
                JSPropertyAttributes.ReadonlyValue);
// Exporting Type as type
prototype.FastAddProperty(
                "type".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Blob @this
                        ? @this.Type
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Blob") ,
                "get type"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Size as size
prototype.FastAddProperty(
                "size".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Blob @this
                        ? @this.Size
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Blob") ,
                "get size"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting ArrayBuffer as arrayBuffer
prototype.FastAddProperty(
                "arrayBuffer".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    a.This is Blob @this
                        ? @this.ArrayBuffer
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Blob") ,
                "get arrayBuffer"),
                null,
                JSPropertyAttributes.ConfigurableProperty);
// Exporting Slice as slice
prototype.FastAddValue("slice".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is Blob @this
                        ? @this.Slice(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Blob")
                        , "slice"
                        ,"function slice() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Text as text
prototype.FastAddValue("text".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is Blob @this
                        ? @this.Text(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Blob")
                        , "text"
                        ,"function text() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Stream as stream
prototype.FastAddValue("stream".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is Blob @this
                        ? @this.Stream(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to Blob")
                        , "stream"
                        ,"function stream() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
