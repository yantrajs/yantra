#r "nuget: YantraJS.Core,1.2.1"
#r "nuget: crockford-base32-core,2.0.0"
using System;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Core.Clr;
using CrockfordBase32;

[DefaultExport]
public class Test
{
    public static async Task<string> Base32(long n)
    {
        await Task.Delay(10);
        CrockfordBase32Encoding en = new CrockfordBase32Encoding();
        return en.Encode((ulong)n, false);
    }
}