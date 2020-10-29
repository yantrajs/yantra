using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core.Generator
{
    public class JSAwaiter : JSPromise, IDisposable
    {

        Thread thread;
        private bool disposedValue;
        private AutoResetEvent main;

        public JSAwaiter(JSAsyncDelegate @delegate, in Arguments a)
            : base(JSUndefined.Value, PromiseState.Pending)
        {
            main = new AutoResetEvent(false);
            this.thread = new Thread(RunAwaiter);
            thread.Start(new JSWeakAwaiter(this, @delegate, a, main));
        }

        private static void RunAwaiter(object p)
        {
            JSWeakAwaiter awaiter = p as JSWeakAwaiter;
            JSAsyncDelegate @delegate = awaiter.@delegate;
            awaiter.@delegate = null;
            try
            {
                @delegate(awaiter, awaiter.a);
                awaiter.Resolve();
            }
            catch (SafeExitException) { 
                // do nothing..
            }
            catch (Exception ex)
            {
                awaiter.Reject(ex);
            }
        }


        public class JSWeakAwaiter
        {
            private AutoResetEvent main;
            public WeakReference<JSAwaiter> awaiter;
            public JSAsyncDelegate @delegate;
            public readonly Arguments a;

            public JSValue result;
            public JSValue error;

            public JSWeakAwaiter(JSAwaiter awaiter, JSAsyncDelegate @delegate, in Arguments a, AutoResetEvent main)
            {
                this.main = main;
                this.a = a;
                this.@delegate = @delegate;
                this.awaiter = new WeakReference<JSAwaiter>(awaiter);
            }
            public void Reject(Exception ex)
            {
                if (awaiter.TryGetTarget(out var a)) {
                    a.Reject(JSException.ErrorFrom(ex));
                }
            }

            public JSValue Await(JSValue value)
            {
                if (value.IsNullOrUndefined)
                {
                    throw JSContext.Current.NewTypeError($"await cannot be called on undefined/null");
                }
                // if it has then method...
                var method = value[KeyStrings.then];
                if (!method.IsFunction)
                {
                    // what to do here.. just return...
                    return value;
                }

                var res = new JSFunction((in Arguments resolve) => {
                    result = resolve.Get1();
                    main.Set();
                    return JSUndefined.Value;
                });
                var rej = new JSFunction((in Arguments reject) => {
                    error = reject.Get1();
                    main.Set();
                    return JSUndefined.Value;
                });

                method.InvokeFunction(new Arguments(a.This, res, rej));

                // block till we get next result...
                try
                {
                    main.WaitOne(Timeout.Infinite);
                } catch (ObjectDisposedException)
                {
                    // do nothing...
                    throw new SafeExitException();
                }
                if (error != null)
                {
                    throw JSException.FromValue(error);
                }
                return result;
            }
            public void Resolve()
            {
                if(awaiter.TryGetTarget(out var a))
                {
                    a.Resolve(this.result);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        main?.Dispose();
                    } catch { }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~JSAwaiter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public delegate void JSAsyncDelegate(in JSAwaiter.JSWeakAwaiter generator, in Arguments a);
    public class JSAsyncFunction : JSFunction
    {
        readonly JSAsyncDelegate @delegate;

        public JSAsyncFunction(JSAsyncDelegate @delegate, string name, string code) :
            base(JSFunction.empty, name, code)
        {
            this.@delegate = @delegate;
        }


        public override JSValue InvokeFunction(in Arguments a)
        {
            return new JSAwaiter(@delegate, a);
        }

    }

}
