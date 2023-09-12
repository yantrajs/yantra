using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Error;

namespace YantraJS.Core.Core.Disposable
{
    public class JSDisposableStack: IDisposable, IAsyncDisposable
    {

        public bool Disposed { get; private set; }

        public bool isAsync { get; private set; }

        public JSValue Error { get; private set; }

        private Stack<(JSValue value,bool async)> stack = new Stack<(JSValue,bool)>();

        public JSDisposableStack()
        {
        
        }

        public void AddDisposableResource(JSValue value, bool async = false)
        {
            if(value.IsNullOrUndefined)
            {
                return;
            }
            this.isAsync |= async;
            this.stack.Push((value, async));
        }

        public JSValue Dispose()
        {
            if (!this.isAsync)
            {
                ((IDisposable)this).Dispose();
                return JSUndefined.Value;
            }
            var task = this.DisposeAsync();
            return task.ToPromise();
        }

        void IDisposable.Dispose()
        {
            while(stack.Count > 0) {
                var (v, a) = stack.Pop();
                if(a)
                {
                    throw JSContext.Current.NewTypeError("Async resource must not be disposed synchronously.");
                }
                try
                {
                    v.InvokeMethod(JSSymbol.dispose);
                } catch (Exception ex)
                {
                    this.Error = new JSSuppressedError(JSError.From(ex), this.Error);
                }
            }

            if (this.Error != null)
            {
                JSException.Throw(this.Error);
            }
        }

        private async Task DisposeAsync()
        {
            while (stack.Count > 0)
            {
                var (v, a) = stack.Pop();
                try
                {
                    if (a)
                    {
                        var r = v.InvokeMethod(JSSymbol.asyncDispose);
                        await JSPromise.Await(r);
                    }
                    else
                    {
                        v.InvokeMethod(JSSymbol.dispose);
                    }
                }
                catch (Exception ex)
                {
                    this.Error = new JSSuppressedError(JSError.From(ex), this.Error);
                }
            }

            if (this.Error != null)
            {
                JSException.Throw(this.Error);
            }

        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await this.DisposeAsync();
        }
    }
}
