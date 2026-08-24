using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Weak { 
partial class JSWeakRef {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSWeakRef(in a)
                            , "WeakRef"
                            , "function WeakRef() { [native code] }"
                            );
                        if (register) {
                            context["WeakRef".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
// Exporting Deref as deref
prototype.FastAddValue("deref".ToKeyString(), new JSFunction(context, (in Arguments a) =>
                    a.This is JSWeakRef @this
                        ? @this.Deref(in a)
                        : throw JSContext.Current.NewTypeError("Failed to convert this to JSWeakRef")
                        , "deref"
                        ,"function deref() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
