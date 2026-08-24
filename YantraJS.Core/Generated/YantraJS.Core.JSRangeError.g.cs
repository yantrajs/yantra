using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSRangeError {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSRangeError(in a)
                            , "RangeError"
                            , "function RangeError() { [native code] }"
                            );
                        if (register) {
                            context["RangeError".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["Error".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
return @class;
}
}
}
