using Esprima.Ast;
using Microsoft.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        [Static("setImmediate")]
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

        private static ConcurrentDictionary<long, CancellationTokenSource> tokens
            = new ConcurrentDictionary<long, CancellationTokenSource>();

        private static long timeouts = 1;

        [Static("setTimeout")]
        public static JSValue SetTimeout(in Arguments a)
        {
            var cancel = new CancellationTokenSource();
            var @this = a.This;
            var (fx, timeout) = a.Get2();
            if (!(fx is JSFunction f))
                throw JSContext.Current.NewTypeError("Argument is not a function");
            var delay = timeout.IsUndefined ? 0 : timeout.IntValue;
            var key = Interlocked.Increment(ref timeouts);
            tokens.AddOrUpdate(key, cancel, (a1,a2) => cancel);
            AsyncPump.Run( async () => {
                try
                {
                    await Task.Delay(delay, cancel.Token);
                    f.f(new Arguments(@this));
                    tokens.TryRemove(key, out var aa);
                } catch (TaskCanceledException) {

                } catch (Exception ex)
                {
                    JSContext.Current.ReportError(ex);
                }
            });
            return new JSBigInt(key);
        }

        [Static("clearTimeout")]
        public static JSValue ClearTimeout(in Arguments a)
        {
            var n = a.Get1().BigIntValue;
            if(tokens.TryRemove(n, out var token))
            {
                token.Cancel();
            }
            return JSUndefined.Value;
        }


    }
}
