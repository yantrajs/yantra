using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSEvalError {
public static new JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new JSEvalError(in a)
                            , "EvalError"
                            , "function EvalError() { [native code] }"
                            );
                        if (register) {
                            context[KeyString.EvalError] = @class;
                        }
                        prototype = @class.prototype;
                        
 var @base = context[KeyString.Error] as JSFunction;
@class.SetPrototypeOf(@base);
prototype.SetPrototypeOf(@base.prototype);
context.EvalError_Prototype = prototype.PrototypeObject;
return @class;
}
}
}
