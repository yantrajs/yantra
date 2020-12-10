#r "nuget: YantraJS.Core,1.0.18"
using System;
using System.IO;
using System.Linq;
using YantraJS;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YInspect {



}

public class YUtil {

    public static JSValue inspect = ClrType.From(typeof(YInspect));

}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {
    module["exports"] = ClrType.From(typeof(YUtil));
}


return (JSModuleDelegate)Module;