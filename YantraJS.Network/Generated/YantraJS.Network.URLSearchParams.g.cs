using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class URLSearchParams {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new URLSearchParams(in a)
                            , "URLSearchParams"
                            , "function URLSearchParams() { [native code] }"
                            );
                        if (register) {
                            context["URLSearchParams".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["KeyValueStore".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
return @class;
}
}
}
