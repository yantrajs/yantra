using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Core.BigInt;
using YantraJS.Core.LightWeight;

namespace YantraJS.Core.Generator
{
    public class JSAwaiter : JSPromise, IDisposable
    {

        private AutoResetEvent main;
        private AutoResetEvent originatorThread;
        // private LightWeightStack<CallStackItem> originalStack;
        // private LightWeightStack<CallStackItem> current;
        private LexicalScope originalStack;
        private LexicalScope current;
        private JSContext context;

        public JSAwaiter(JSAsyncDelegate @delegate, in Arguments a)
            : base(JSUndefined.Value, PromiseState.Pending)
        {
            context = JSContext.Current;
            main = new AutoResetEvent(false);
            originatorThread = new AutoResetEvent(false);
            JSThreadPool.Queue(RunAwaiter, new JSWeakAwaiter(this, @delegate, a, main));
            originalStack = context.Stack._Top;
            // current = new LightWeightStack<CallStackItem>(originalStack);
            // context.Switch(current);
            main.Set();
            originatorThread.WaitOne();
        }

        private JSValue error;

        public void Finish(JSValue value)
        {
            this.Resolve(value);
            context.Stack.Switch(originalStack);
            originatorThread.Set();
            this.Dispose();
        }

        public void Fail(JSValue value)
        {
            this.Reject(value);
            context.Stack.Switch(originalStack);
            originatorThread.Set();
            this.Dispose();
        }

        public JSValue Await(JSValue value)
        {
            error = null;
            // setup the promise...
            if (value.IsNullOrUndefined)
            {
                throw JSContext.Current.NewTypeError($"await cannot be called on undefined or null");
            }

            JSFunctionDelegate successFx = (in Arguments a) =>
            {
                result = a.Get1();
                originalStack = context.Stack.Switch(current);
                main.Set();
                originatorThread.WaitOne();
                return JSUndefined.Value;
            };

            JSFunctionDelegate failFx = (in Arguments a) => {
                error = a.Get1();
                originalStack = context.Stack.Switch(current);
                main.Set();
                originatorThread.WaitOne();
                return JSUndefined.Value;
            };

            // is it a native promise
            if (value is JSPromise promise)
            {
                result = promise.Then(successFx, failFx);
            }
            else
            {

                var method = value[KeyStrings.then];
                if (!method.IsFunction)
                {
                    this.result = value;
                    return value;
                }
                var success = new JSFunction(successFx);

                var fail = new JSFunction(failFx);

                result = method.InvokeFunction(new Arguments(value, success, fail));
            }


            if (error != null)
            {
                this.Reject(error);
                originatorThread.Set();
                return error;
            }

            current = context.Stack.Switch(originalStack);
            originatorThread.Set();
            main.WaitOne();

            return result;
        }

        private static void RunAwaiter(object p)
        {
            JSWeakAwaiter awaiter = p as JSWeakAwaiter;
            JSAsyncDelegate @delegate = awaiter.@delegate;
            awaiter.@delegate = null;
            try
            {
                SynchronizationContext.SetSynchronizationContext(awaiter.context);
                awaiter.main.WaitOne();
                var r = @delegate(awaiter, awaiter.a);
                awaiter.Finish(r);
            }
            catch (SafeExitException) { 
                // do nothing..
            }
            catch (Exception ex)
            {
                awaiter.Fail(ex);
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
