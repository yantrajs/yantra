#r "nuget: YantraJS.Core,1.2.1"
#r "nuget: crockford-base32-core,2.0.0"
using System;
using YantraJS.Core;
using YantraJS.Core.Clr;
using CrockfordBase32;

[DefaultExport]
public class Test
{
    public static JSValue Base32(in Arguments a)
    {
        CrockfordBase32Encoding en = new CrockfordBase32Encoding();
        return en.Encode((ulong)a.Get1().BigIntValue, false).Marshal();
    }
}