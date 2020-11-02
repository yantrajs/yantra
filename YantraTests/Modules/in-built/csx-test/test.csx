#r "nuget: YantraJS.Core,1.0.1-CI-20201028-141018"
#r "nuget: crockford-base32-core,2.0.0"
using System;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Clr;
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