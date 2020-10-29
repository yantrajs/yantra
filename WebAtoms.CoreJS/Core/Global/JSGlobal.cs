using Esprima.Ast;
using Microsoft.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core.BigInt;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class JSGlobalStatic
    {
        [Static("Infinity")]
        public static JSNumber Infinity = JSNumber.PositiveInfinity;

        [Static("NaN")]
        public static JSNumber NaN = JSNumber.NaN;

        [Static("decodeURI", Length = 1)]
        public static JSValue DecodeURI(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(UriHelper.DecodeURI(f));
        }

        [Static("decodeURIComponent", Length = 1)]
        public static JSValue DecodeURIComponent(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.UnescapeDataString(f));
        }

        [Static("eval", Length = 1)]
        public static JSValue Eval(in Arguments a)
        {
            var f = a.Get1();
            if (!f.IsString)
                return f;
            var text = f.ToString();
            return CoreScript.Evaluate(text, null);
        }

        [Static("encodeURI", Length = 1)]
        public static JSValue EncodeURI(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeUriString(f));

        }

        [Static("encodeURIComponent", Length = 1)]
        public static JSValue EncodeURIComponent(in Arguments a)
        {
            var f = a.Get1().ToString();
            return new JSString(Uri.EscapeDataString(f));
        }

        [Static("isFinite", Length = 1)]
        public static JSValue IsFinite(in Arguments a)
        {
            return JSNumberStatic.IsFinite(a);
        }

        [Static("isNaN", Length = 1)]
        public static JSValue IsNaN(in Arguments a)
        {
            return JSNumberStatic.IsNaN(a);
        }

        [Static("parseFloat", Length = 1)]
        public static JSValue ParseFloat(in Arguments a)
        {
            return JSNumberStatic.ParseFloat(a);
        }

        [Static("parseInt", Length = 2)]
        public static JSValue ParseInt(in Arguments a)
        {
            return JSNumberStatic.ParseInt(a);
        }

        [Static("setImmediate", Length = 1)]
        public static JSValue SetImmediate(in Arguments a)
        {
            var @this = a.This;
            var fx = a.Get1();
            if (!(fx is JSFunction f))
                throw JSContext.Current.NewTypeError("Argument is not a function");
            AsyncPump.Run(() => {
                f.f(new Arguments(@this));
                return Task.CompletedTask;
            });
            return JSUndefined.Value;
        }

        private static long timeouts = 1;

        private static long intervals = 1;

        [Static("setInterval", Length = 2)]
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

        [Static("clearInterval", Length = 1)]
        public static JSValue ClearInterval(in Arguments a)
        {
            var n = a.Get1().BigIntValue;
            JSContext.Current.ClearInterval(n);
            return JSUndefined.Value;
        }

        [Static("setTimeout", Length = 2)]
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

        [Static("clearTimeout", Length = 1)]
        public static JSValue ClearTimeout(in Arguments a)
        {
            var n = a.Get1().BigIntValue;
            var context = JSContext.Current;
            context.ClearTimeout(n);
            return JSUndefined.Value;
        }


    }
}
