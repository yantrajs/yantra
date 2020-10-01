using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class JSGlobalStatic
    {
        [Static("decodeURI")]
        public static JSValue DecodeURI(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(UriHelper.DecodeURI(f));
        }

        [Static("decodeURIComponent")]
        public static JSValue DecodeURIComponent(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.UnescapeDataString(f));
        }

        [Static("eval")]
        public static JSValue Eval(in Arguments a)
        {
            var f = a.Get1();
            if (!f.IsString)
                return f;
            var text = f.ToString();
            return CoreScript.Evaluate(text);
        }

        [Static("encodeURI")]
        public static JSValue EncodeURI(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeUriString(f));

        }

        [Static("encodeURIComponent")]
        public static JSValue EncodeURIComponent(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeDataString(f));
        }

        [Static("isFinite")]
        public static JSValue IsFinite(in Arguments a)
        {
            return JSNumberStatic.IsFinite(a);
        }

        [Static("isNaN")]
        public static JSValue IsNaN(in Arguments a)
        {
            return JSNumberStatic.IsNaN(a);
        }

        [Static("parseFloat")]
        public static JSValue ParseFloat(in Arguments a)
        {
            return JSNumberStatic.ParseFloat(a);
        }

        [Static("parseInt")]
        public static JSValue ParseInt(in Arguments a)
        {
            return JSNumberStatic.ParseInt(a);
        }

    }
}
