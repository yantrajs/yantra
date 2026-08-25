using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSProxy {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSFunction(context, (in Arguments a) => JSProxy.Constructor(in a)
                            , "Proxy"
                            , "function Proxy() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.Proxy] = @class;
                        }
                        prototype = @class.prototype;
                        
context.Proxy_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
