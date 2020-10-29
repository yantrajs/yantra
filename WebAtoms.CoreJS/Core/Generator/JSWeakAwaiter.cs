using System;
using System.Threading;

namespace WebAtoms.CoreJS.Core.Generator
{
    public class JSWeakAwaiter
    {
        private AutoResetEvent main;
        public WeakReference<JSAwaiter> awaiter;
        public JSAsyncDelegate @delegate;
        public readonly Arguments a;

        public JSValue result;
        public JSValue error;
        public readonly SynchronizationContext SynchronizationContext;

        public JSWeakAwaiter(JSAwaiter awaiter, JSAsyncDelegate @delegate, in Arguments a, AutoResetEvent main)
        {
            this.main = main;
            this.a = a;
            this.@delegate = @delegate;
            this.awaiter = new WeakReference<JSAwaiter>(awaiter);
            this.SynchronizationContext = SynchronizationContext.Current;
        }
        public void Reject(Exception ex)
        {
            if (awaiter.TryGetTarget(out var a))
            {
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
                this.result = value;
                return value;
            }

            if (value is JSPromise p)
            {
                if (p.state != JSPromise.PromiseState.Pending)
                    return p;
            }

            var finished = false;

            var res = new JSFunction((in Arguments resolve) =>
            {
                result = resolve.Get1();
                main.Set();
                finished = true;
                return JSUndefined.Value;
            });
            var rej = new JSFunction((in Arguments reject) =>
            {
                error = reject.Get1();
                main.Set();
                finished = true;
                return JSUndefined.Value;
            });

            method.InvokeFunction(new Arguments(value, res, rej));

            // block till we get next result...
            try
            {
                while (!finished)
                {
                    if(main.WaitOne(TimeSpan.FromSeconds(100)))
                        break;
                }
            }
            catch (ObjectDisposedException)
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
        public void Resolve(JSValue value)
        {
            if (awaiter.TryGetTarget(out var a))
            {
                a.Resolve(value);
            }
        }
    }
}
