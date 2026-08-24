using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSSymbol {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSSymbol.Constructor(in a)
                            , "Symbol"
                            , "function Symbol() { [native code] }"
                            );
                        if (register) {
                            context["Symbol".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting asyncDispose as asyncDispose
@class.FastAddProperty(
                "asyncDispose".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.asyncDispose),
                "get asyncDispose"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.asyncDispose = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "asyncDispose");
                    return JSUndefined.Value;
                },
                "set asyncDispose"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting dispose as dispose
@class.FastAddProperty(
                "dispose".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.dispose),
                "get dispose"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.dispose = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "dispose");
                    return JSUndefined.Value;
                },
                "set dispose"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting asyncIterator as asyncIterator
@class.FastAddProperty(
                "asyncIterator".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.asyncIterator),
                "get asyncIterator"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.asyncIterator = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "asyncIterator");
                    return JSUndefined.Value;
                },
                "set asyncIterator"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting hasInstance as hasInstance
@class.FastAddProperty(
                "hasInstance".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.hasInstance),
                "get hasInstance"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.hasInstance = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "hasInstance");
                    return JSUndefined.Value;
                },
                "set hasInstance"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting isConcatSpreadable as isConcatSpreadable
@class.FastAddProperty(
                "isConcatSpreadable".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.isConcatSpreadable),
                "get isConcatSpreadable"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.isConcatSpreadable = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "isConcatSpreadable");
                    return JSUndefined.Value;
                },
                "set isConcatSpreadable"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting iterator as iterator
@class.FastAddProperty(
                "iterator".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.iterator),
                "get iterator"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.iterator = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "iterator");
                    return JSUndefined.Value;
                },
                "set iterator"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting match as match
@class.FastAddProperty(
                "match".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.match),
                "get match"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.match = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "match");
                    return JSUndefined.Value;
                },
                "set match"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting replace as replace
@class.FastAddProperty(
                "replace".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.replace),
                "get replace"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.replace = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "replace");
                    return JSUndefined.Value;
                },
                "set replace"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting search as search
@class.FastAddProperty(
                "search".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.search),
                "get search"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.search = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "search");
                    return JSUndefined.Value;
                },
                "set search"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting species as species
@class.FastAddProperty(
                "species".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.species),
                "get species"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.species = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "species");
                    return JSUndefined.Value;
                },
                "set species"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting split as split
@class.FastAddProperty(
                "split".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.split),
                "get split"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.split = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "split");
                    return JSUndefined.Value;
                },
                "set split"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting toPrimitive as toPrimitive
@class.FastAddProperty(
                "toPrimitive".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.toPrimitive),
                "get toPrimitive"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.toPrimitive = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "toPrimitive");
                    return JSUndefined.Value;
                },
                "set toPrimitive"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting toStringTag as toStringTag
@class.FastAddProperty(
                "toStringTag".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.toStringTag),
                "get toStringTag"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.toStringTag = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "toStringTag");
                    return JSUndefined.Value;
                },
                "set toStringTag"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting unscopables as unscopables
@class.FastAddProperty(
                "unscopables".ToKeyString(),
                new JSFunction(context, (in Arguments a) =>
                    ClrProxy.Marshal(JSSymbol.unscopables),
                "get unscopables"),
                new JSFunction(context, (in Arguments a) => {
                    JSSymbol.unscopables = JSValueToClrConverter.GetAsOrThrow<YantraJS.Core.JSSymbol>(a[0], "unscopables");
                    return JSUndefined.Value;
                },
                "set unscopables"),
                JSPropertyAttributes.ConfigurableProperty);
// Exporting For as for
@class.FastAddValue("for".ToKeyString(), new JSFunction(context, JSSymbol.For, "for" ,"function for() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
