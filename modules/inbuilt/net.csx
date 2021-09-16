#r "nuget: YantraJS.Core,1.1.106"
using System;
using System.IO;
using System.Linq;
using YantraJS;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class YNet {


    public static JSValue IsIP(JSValue input) {
        if(!input.IsString)
            throw JSContext.CurrentContext.NewTypeError($"parameter input is not a string");
        if(!System.Net.IPAddress.TryParse(input.ToString(), out var ip))
            return JSNumber.Zero;
        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            return new JSNumber(6);
        return new JSNumber(4);
    }
}

static void Module(JSValue exports, JSValue require, JSValue module, string __filename, string __dirname) {
    module["exports"] = ClrType.From(typeof(YNet));
}


return (JSModuleDelegate)Module;