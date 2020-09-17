using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{


    public delegate void JSPromiseDelegate(
        Action<JSValue> resolve, 
        Action<JSValue> reject);

    [JSRuntime(typeof(JSPromiseStatic), typeof(JSPromisePrototype))]
    public class JSPromise: JSObject
    {

        internal enum PromiseState
        {
            Pending,
            Resolved,
            Rejected
        }

        internal PromiseState state = PromiseState.Pending;

        private List<JSFunctionDelegate> thenList;
        private List<JSFunctionDelegate> rejectList;
        JSFunction resolveFunction;
        JSFunction rejectFunction;
        private JSValue result;

        public JSPromise(JSValue @delegate) :
            base(JSContext.Current.PromisePrototype)
        {

            // to improve speed of promise, we will add then/catch here...


            resolveFunction = new JSFunction((_, __) => Resolve(__.GetAt(0)));
            rejectFunction = new JSFunction((_, __) => Reject(__.GetAt(0)));
            @delegate.InvokeFunction(this, resolveFunction, rejectFunction);

        }

        public JSPromise(JSPromiseDelegate @delegate) :
            base(JSContext.Current.PromisePrototype)
        {
            @delegate(p => Resolve(p), p => Reject(p) );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JSValue Resolve(JSValue value)
        {
            if (value == this)
            {
                Reject(JSContext.Current.NewTypeError("A promise cannot be resolved with itself").Error);
                return JSUndefined.Value;
            }

            // get then...
            if(value.IsObject)
            {
                var then = value["then"];
                if (then.IsFunction)
                {
                    // do what....
                    try
                    {
                        then.InvokeFunction(value, resolveFunction, rejectFunction);
                    } 
                    catch(JSException jse)
                    {
                        Reject(jse.Error);
                    }
                    catch (Exception ex)
                    {
                        Reject(new JSString(ex.ToString()));
                    }
                    return JSUndefined.Value;
                }
            }

            this.state = PromiseState.Resolved;
            this.result = value;

            var thenList = this.thenList;
            if (thenList != null) {
                this.thenList = null;
                foreach (var t in thenList)
                {
                    t(this, value);
                }
            }

            return JSUndefined.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JSValue Reject(JSValue value)
        {
            this.state = PromiseState.Rejected;
            this.result = value;

            var rejectList = this.rejectList;
            if (rejectList != null)
            {
                this.rejectList = null;
                foreach (var t in rejectList)
                {
                    t(this, value);
                }
            }
            return JSUndefined.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Then(JSFunctionDelegate d)
        {
            if (this.state == PromiseState.Resolved)
            {
                d(this, this.result);
                return;
            }
            if (this.state != PromiseState.Pending)
                return;
            var thenList = this.thenList ?? (this.thenList = new List<JSFunctionDelegate>());
            thenList.Add(d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Catch(JSFunctionDelegate d)
        {
            if (this.state == PromiseState.Rejected)
            {
                d(this, this.result);
                return;
            }
            if (this.state != PromiseState.Pending)
                return;
            var thenList = this.rejectList ?? (this.rejectList = new List<JSFunctionDelegate>());
            thenList.Add(d);
        }

    }
}
