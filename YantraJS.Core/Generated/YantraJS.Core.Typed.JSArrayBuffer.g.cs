using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Core.Typed { 
partial class JSArrayBuffer {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSArrayBuffer(in a)
                            , "ArrayBuffer"
                            , "function ArrayBuffer() { [native code] }"
                            );
                        if (register) {
                            context["ArrayBuffer".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
return @class;
}
}
}
