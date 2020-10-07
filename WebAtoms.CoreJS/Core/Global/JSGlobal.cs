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
            return CoreScript.Evaluate(text);
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

        private static ConcurrentDictionary<long, CancellationTokenSource> intervalTokens
            = new ConcurrentDictionary<long, CancellationTokenSource>();

        private static long intervals = 1;

        [Static("setInterval", Length = 2)]
        public static JSValue SetInterval(in Arguments a)
        {
            var cancel = new CancellationTokenSource();
            var @this = a.This;
            var (fx, timeout) = a.Get2();
            if (!(fx is JSFunction f))
                throw JSContext.Current.NewTypeError("Argument is not a function");
            var delay = timeout.IsUndefined ? 0 : timeout.IntValue;
            var key = Interlocked.Increment(ref intervals);
            intervalTokens.AddOrUpdate(key, cancel, (a1, a2) => cancel);
            JSContext current = JSContext.Current;

            Func<Task> task = async () => {
                try
                {
                    while (!cancel.IsCancellationRequested)
                    {
                        try
                        {
                            await Task.Delay(delay, cancel.Token);
                            f.f(new Arguments(@this));
                        }
                        catch (Exception ex)
                        {
                            current.ReportError(ex);
                        }
                    }
                } catch (TaskCanceledException)
                {

                } catch (Exception ex)
                {
                    current.ReportError(ex);
                }
                if(current.waitTask != null)
                {
                    await current.waitTask;
                }
            };

            current.waitTask = task();

            return new JSBigInt(key);
        }

        [Static("clearInterval", Length = 1)]
        public static JSValue ClearInterval(in Arguments a)
        {
            var n = a.Get1().BigIntValue;
            if (intervalTokens.TryRemove(n, out var token))
            {
                token.Cancel();
            }
            return JSUndefined.Value;
        }


        private static ConcurrentDictionary<long, CancellationTokenSource> tokens
            = new ConcurrentDictionary<long, CancellationTokenSource>();

        [Static("setTimeout", Length = 2)]
        public static JSValue SetTimeout(in Arguments a)
        {
            var cancel = new CancellationTokenSource();
            var @this = a.This;
            var (fx, timeout) = a.Get2();
            var current = JSContext.Current;
            if (!(fx is JSFunction f))
                throw current.NewTypeError("Argument is not a function");
            var delay = timeout.IsUndefined ? 0 : timeout.IntValue;
            var key = Interlocked.Increment(ref timeouts);

            // LinkedListNode<Task> node = current.waitTasks.AddLast((Task)null);
            tokens.AddOrUpdate(key, cancel, (a1, a2) => a2);

            var oldWait = current.waitTask;

            async Task RunTask()
            {
                try
                {
                    await Task.Delay(delay, cancel.Token);
                    f.f(new Arguments(@this));
                }
                catch (TaskCanceledException)
                {

                } catch (Exception ex)
                {
                    current.ReportError(ex);
                }
                tokens.TryRemove(key, out var aa);
                if(oldWait != null)
                {
                    await oldWait;
                }
            }

            current.waitTask = RunTask();

            return new JSBigInt(key);
        }

        [Static("clearTimeout", Length = 1)]
        public static JSValue ClearTimeout(in Arguments a)
        {
            var n = a.Get1().BigIntValue;
            if(tokens.TryRemove(n, out var token))
            {
                token.Cancel();
                // JSContext.Current.waitTasks.Remove(token.node);
            }
            return JSUndefined.Value;
        }


    }
}
