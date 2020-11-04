using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS.Core.Generator
{
    public class JSAwaiter : JSPromise, IDisposable
    {

        private AutoResetEvent main;

        public JSAwaiter(JSAsyncDelegate @delegate, in Arguments a)
            : base(JSUndefined.Value, PromiseState.Pending)
        {
            main = new AutoResetEvent(false);
            JSThreadPool.Queue(RunAwaiter, new JSWeakAwaiter(this, @delegate, a, main));
        }

        private static void RunAwaiter(object p)
        {
            JSWeakAwaiter awaiter = p as JSWeakAwaiter;
            JSAsyncDelegate @delegate = awaiter.@delegate;
            awaiter.@delegate = null;
            try
            {
                SynchronizationContext.SetSynchronizationContext(awaiter.SynchronizationContext);
                var r = @delegate(awaiter, awaiter.a);
                awaiter.Resolve(r);
            }
            catch (SafeExitException) { 
                // do nothing..
            }
            catch (Exception ex)
            {
                awaiter.Reject(ex);
            }
        }


        protected virtual void OnDispose()
        {
            try
            {
                main?.Dispose();
            }
            catch { }
            main = null;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~JSAwaiter()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            OnDispose();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            OnDispose();
            GC.SuppressFinalize(this);
        }
    }
}
