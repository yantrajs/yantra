using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public class JSGlobalStatic
    {
        [Static("decodeURI")]
        public static JSValue DecodeURI(JSValue t, JSValue[] a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.UnescapeDataString(f));
        }

        [Static("decodeURIComponent")]
        public static JSValue DecodeURIComponent(JSValue t, JSValue[] a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.UnescapeDataString(f));
        }

        [Static("eval")]
        public static JSValue Eval(JSValue t, JSValue[] a)
        {
            var f = a.Get1();
            if (!f.IsString)
                return f;
            var text = f.ToString();
            return CoreScript.Evaluate(text);
        }

        [Static("encodeURI")]
        public static JSValue EncodeURI(JSValue t, JSValue[] a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeUriString(f));

        }

        [Static("encodeURIComponent")]
        public static JSValue EncodeURIComponent(JSValue t, JSValue[] a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeDataString(f));
        }

        [Static("isFinite")]
        public static JSValue IsFinite(JSValue t, JSValue[] a)
        {
            return JSNumberStatic.IsFinite(t, a);
        }

        [Static("isNaN")]
        public static JSValue IsNaN(JSValue t, JSValue[] a)
        {
            return JSNumberStatic.IsNaN(t, a);
        }

        [Static("parseFloat")]
        public static JSValue ParseFloat(JSValue t, JSValue[] a)
        {
            return JSNumberStatic.ParseFloat(t, a);
        }

        [Static("parseInt")]
        public static JSValue ParseInt(JSValue t, JSValue[] a)
        {
            return JSNumberStatic.ParseInt(t, a);
        }

    }
}
