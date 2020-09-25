using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Date
{
    public static class JSDateStatic
    {
        [Static("UTC")]
        internal static JSValue UTC(in Arguments a)
        {
            return new JSDate(DateTime.UtcNow);
        }

        [Static("now")]
        internal static JSValue Now(in Arguments a)
        {
            return new JSDate(DateTime.UtcNow);
        }

        [Static("parse")]
        internal static JSValue Parse(in Arguments a)
        {
            return new JSDate(DateTime.Parse(a.Get1().ToString()));
        }

        [Prototype("getYear")]
        internal static JSValue GetYear(in Arguments a)
        {
            if (!(a.This is JSDate d))
                throw JSContext.Current.NewTypeError("Method Date.prototype.getYear called on incompatible receiver");
            return new JSNumber(d.value.Year - 2000);
        }
    }
}
