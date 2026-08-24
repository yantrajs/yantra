using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class URL {
public static JSFunction CreateClass(JSContext context, bool register = true) {

                        JSObject prototype = null;
                        var @class = new JSClassFunction(context, (in Arguments a) => new URL(in a)
                            , "URL"
                            , "function URL() { [native code] }"
                            );
                        if (register) {
                            context["URL".ToKeyString()] = @class;
                        }
                        prototype = @class.prototype;
                        
return @class;
}
}
}
