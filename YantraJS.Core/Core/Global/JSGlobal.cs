using Microsoft.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS.Core.BigInt;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Intl;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{
    [JSFunctionGenerator("Globals", Globals = true)]
    public partial class JSGlobalStatic
    {
        [JSExport("Infinity")]
        public static JSNumber Infinity = JSNumber.PositiveInfinity;

        [JSExport("NaN")]
        public static JSNumber NaN = JSNumber.NaN;

        [JSExport("Intl")]
        public static JSValue Intl = ClrType.From(typeof(JSIntl));

        [JSExport("decodeURI", Length = 1)]
        public static JSValue DecodeURI(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(UriHelper.DecodeURI(f));
        }

        [JSExport("decodeURIComponent", Length = 1)]
        public static JSValue DecodeURIComponent(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.UnescapeDataString(f));
        }

        [JSExport("eval", Length = 1)]
        public static JSValue Eval(in Arguments a)
        {
            var f = a.Get1();
            if (!f.IsString)
                return f;
            var text = (f as JSString).value;
            string location = null;
            JSContext.Current.DispatchEvalEvent(ref text, ref location);
            return CoreScript.Evaluate(text, null);
        }

        [JSExport("encodeURI", Length = 1)]
        public static JSValue EncodeURI(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeUriString(f));

        }

        [JSExport("encodeURIComponent", Length = 1)]
        public static JSValue EncodeURIComponent(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeDataString(f));
        }

        [JSExport("isFinite", Length = 1)]
        public static JSValue IsFinite(in Arguments a)
        {
            return JSNumber.IsFinite(a);
        }

        [JSExport("isNaN", Length = 1)]
        public static JSValue IsNaN(in Arguments a)
        {
            return double.IsNaN( a.Get1().DoubleValue) 
                ? JSBoolean.True
                : JSBoolean.False;
        }

        [JSExport("parseFloat", Length = 1)]
        public static JSValue ParseFloat(in Arguments a)
        {
            return JSNumber.ParseFloat(a);
        }

        [JSExport("parseInt", Length = 2)]
        public static JSValue ParseInt(in Arguments a)
        {
            return JSNumber.ParseInt(a);
        }

        [JSExport("setImmediate", Length = 1)]
        public static JSValue SetImmediate(in Arguments a)
        {
            var @this = a.This;
            var fx = a.Get1();
            if (!(fx is JSFunction f))
                throw JSContext.Current.NewTypeError("Argument is not a function");
            //AsyncPump.Run(() => {
            //    f.f(new Arguments(@this));
            //    return Task.CompletedTask;
            //});
            var c = JSContext.Current;
            SynchronizationContext.Current.Post((_1) => { 
                try {
                    f.f(new Arguments(_1 as JSValue));
                } catch (Exception ex)
                {
                    c.ReportError(ex);
                }
            }, @this);
            return JSUndefined.Value;
        }

        [JSExport("setInterval", Length = 2)]
        public static JSValue SetInterval(in Arguments a)
        {
            var @this = a.This;
            var (fx, timeout) = a.Get2();
            if (!(fx is JSFunction f))
                throw JSContext.Current.NewTypeError("Argument is not a function");
            var delay = timeout.IsUndefined ? 0 : timeout.IntValue;
            var key = JSContext.Current.SetInterval(delay, f, a);
            return new JSBigInt(key);
        }

        [JSExport("clearInterval", Length = 1)]
        public static JSValue ClearInterval(in Arguments a)
        {
            var n = a.Get1().BigIntValue;
            JSContext.Current.ClearInterval(n);
            return JSUndefined.Value;
        }

        [JSExport("setTimeout", Length = 2)]
        public static JSValue SetTimeout(in Arguments a)
        {
            var context = JSContext.Current;
            var (fx, timeout) = a.Get2();
            var current = JSContext.Current;
            if (!(fx is JSFunction f))
                throw current.NewTypeError("Argument is not a function");
            var delay = timeout.IsUndefined ? 0 : timeout.IntValue;
            var key = context.PostTimeout(delay, f, a);
            return new JSBigInt(key);
        }

        [JSExport("clearTimeout", Length = 1)]
        public static JSValue ClearTimeout(in Arguments a)
        {
            var n = a.Get1().BigIntValue;
            var context = JSContext.Current;
            context.ClearTimeout(n);
            return JSUndefined.Value;
        }


    }
}
