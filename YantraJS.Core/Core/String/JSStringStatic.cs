using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    public partial class JSString
    {

        [JSExport("fromCharCode", Length = 1)]
        internal static JSValue FromCharCode(in Arguments a)
        {
            if (a.Length == 0)
                return new JSString(string.Empty);
            var al = a.Length;
            StringBuilder sb = new StringBuilder(al);
            for(var ai = 0; ai < al; ai++)
            {
                var ch = a.GetAt(ai);
                sb.Append((char)ch.IntValue);
            }
            return new JSString(sb.ToString());
        }

        [JSExport("fromCodePoint", Length = 1)]
        internal static JSValue FromCodePoint(in Arguments a) {
            if (a.Length == 0)
                return new JSString(string.Empty);
            var len = a.Length;
            var result = new StringBuilder(len);

            for (var i = 0; i < len; i++) {
                var item = a.GetAt(i);
                var codePointDouble = item.DoubleValue;
                int codePoint = (int)codePointDouble;
                if (codePoint < 0 || codePoint > 0x10FFFF || (double)codePoint != codePointDouble)
                    throw JSContext.Current.NewRangeError($"Invalid code point {codePointDouble}");
                if (codePoint <= 65535)
                    result.Append((char)codePoint);
                else
                {
                    result.Append((char)((codePoint - 65536) / 1024 + 0xD800));
                    result.Append((char)((codePoint - 65536) % 1024 + 0xDC00));
                }

            }
            return new JSString(result.ToString());
        }

        [JSExport("raw", Length = 1)]
        internal static JSValue Raw(in Arguments a)
        {
            var template = a.Get1();
            if (!(template is JSObject))
                throw JSContext.Current.NewTypeError($"Cannot convert undefined or null to object");
            
            var raw = template[KeyStrings.raw];
            if (! (raw.IsString || raw.IsArray)) {
                throw JSContext.Current.NewTypeError($"Cannot convert undefined or null to object");
            }
            var len = raw.Length;
            if (len <= 0)
                return new JSString(string.Empty);
            
            var result = new StringBuilder(len);
            for (uint i = 0; i < len; i++)
            {
                var item = raw[i];
                result.Append(item.ToString());
                var substitutionIndex = i + 1;
                if (i < len - 1 && substitutionIndex < a.Length) {
                    item = a.GetAt((int)substitutionIndex);
                    result.Append(item.ToString());
                }
            }
            return new JSString(result.ToString());
        }
    }
}
