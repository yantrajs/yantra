using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSTypeError {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSTypeError(in a)
                            , "TypeError"
                            , "function TypeError() { [native code] }"
                            );
                        if (register) {
                            context["TypeError".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context["Error".ToKeyString()] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
return @class;
}
}
}
