using System;
using System.Diagnostics;
using System.Net.Cache;
using System.Threading;
using Trace = System.Diagnostics.Debug;

namespace YantraJS.Core.Generator
{
    public class JSWeakAwaiter
    {
        public WeakReference<JSAwaiter> awaiter;
        internal JSAsyncDelegate @delegate;
        internal Arguments a;
        internal SynchronizationContext context;
        internal AutoResetEvent main;

        public JSWeakAwaiter(
            JSAwaiter awaiter, 
            JSAsyncDelegate @delegate, 
            in Arguments a,
            AutoResetEvent main)
        {
            this.awaiter = new WeakReference<JSAwaiter>(awaiter);
            this.main = main;
            this.@delegate = @delegate;
            this.a = a;
            this.context = SynchronizationContext.Current;
        }

        public void Fail(Exception ex)
        {
            if (awaiter.TryGetTarget(out var a))
            {
                a.Fail(JSException.ErrorFrom(ex));
            }
        }

        public JSValue Await(JSValue value)
        {
            if (!awaiter.TryGetTarget(out var a))
            {
                throw new ObjectDisposedException("Awaiter has been disposed");
            }
            return a.Await(value);
            
            //Trace.WriteLine($"Entering await.. !!");
            //if (value.IsNullOrUndefined)
            //{
            //    throw JSContext.Current.NewTypeError($"await cannot be called on undefined/null");
            //}
            //// if it has then method...
            //var method = value[KeyStrings.then];
            //if (!method.IsFunction)
            //{
            //    // what to do here.. just return...
            //    Trace.WriteLine($"Not a promise !!");
            //    this.result = value;
            //    return value;
            //}

            ////if (value is JSPromise p)
            ////{
            ////    if (p.state == JSPromise.PromiseState.Resolved)
            ////        return p.result;
            ////    if (p.state == JSPromise.PromiseState.Rejected)
            ////        throw JSException.FromValue(p.result);
            ////}

            //var finished = false;

            //var res = new JSFunction((in Arguments resolve) =>
            //{
            //    result = resolve.Get1();
            //    main.Set();
            //    finished = true;
            //    return JSUndefined.Value;
            //});
            //var rej = new JSFunction((in Arguments reject) =>
            //{
            //    error = reject.Get1();
            //    main.Set();
            //    finished = true;
            //    return JSUndefined.Value;
            //});

            //result = method.InvokeFunction(new Arguments(value, res, rej));

            //// block till we get next result...
            //try
            //{
            //    while (!finished)
            //    {
            //        Trace.WriteLine($"Entering wait.. !!");
            //        main.WaitOne(TimeSpan.FromSeconds(5));
            //    }
            //}
            //catch (ObjectDisposedException)
            //{
            //    Trace.WriteLine($"Awaiter is disposed !!");
            //    // do nothing...
            //    throw new SafeExitException();
            //}
            //if (error != null)
            //{
            //    throw JSException.FromValue(error);
            //}
            //return result;
        }
        public void Finish(JSValue value)
        {
            if (awaiter.TryGetTarget(out var a))
            {
                a.Finish(value);
            }
        }
    }
}
