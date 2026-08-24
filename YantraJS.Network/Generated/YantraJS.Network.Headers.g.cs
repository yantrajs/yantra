using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class Headers {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new Headers(in a)
                            , "Headers"
                            , "function Headers() { [native code] }"
                            );
                        if (register) {
                            context["Headers".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["KeyValueStore".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
return @class;
}
}
}
