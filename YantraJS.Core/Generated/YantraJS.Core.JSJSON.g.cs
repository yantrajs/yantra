using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
namespace YantraJS.Core { 
partial class JSJSON {
public static JSObject CreateClass(JSContext context, bool register = true) {

                    var @class = new JSObject();
                    if (register) {
                        context["JSON".ToKeyString()] = @class;
                    }
                
// Exporting Parse as parse
@class.FastAddValue("parse".ToKeyString(), new JSFunction(context, JSJSON.Parse, "parse" ,"function parse() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
// Exporting Stringify as stringify
@class.FastAddValue("stringify".ToKeyString(), new JSFunction(context, JSJSON.Stringify, "stringify" ,"function stringify() { [native] }", createPrototype: false), JSPropertyAttributes.ConfigurableValue);
return @class;
}
}
}
