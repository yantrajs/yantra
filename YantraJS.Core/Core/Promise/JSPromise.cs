using Microsoft.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Core.Clr;
using YantraJS.Core.Runtime;
using YantraJS.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core
{


    public delegate void JSPromiseDelegate(
        Action<JSValue> resolve, 
        Action<JSValue> reject);

    /// <summary>
    /// 
    /// </summary>
    [JSRuntime(typeof(JSPromiseStatic), typeof(JSPromisePrototype))]
    public class JSPromise : JSObject
    {


        internal enum PromiseState
        {
            Pending,
            Resolved,
            Rejected
        }

        internal PromiseState state = PromiseState.Pending;

        private List<Action> thenList;
        private List<Action> rejectList;
        JSFunction resolveFunction;
        JSFunction rejectFunction;
        internal JSValue result = JSUndefined.Value;

        static long nextPromiseID = 1;

        long promiseID;
        ConcurrentDictionary<long, JSPromise> pending;

        /// <summary>
        /// .Net removes promises aggressively via
        /// garbage collection... so all promises
        /// till resolved/failed are stored in 
        /// global list
        /// </summary>
        private void  RegisterPromise()
        {
            promiseID = Interlocked.Increment(ref nextPromiseID);

            pending = JSContext.Current.PendingPromises;
            pending.TryAdd(promiseID, this);
        }


        /// <summary>
        /// Promise must stay alive till resolved...
        /// </summary>
        /// <param name="value"></param>
        public JSPromise(Task<JSValue> value)
            : base(JSContext.Current.PromisePrototype)
        {
            RegisterPromise();
            value.ContinueWith((t) => {
                if (t.IsCompleted)
                {
                    Resolve(t.Result);
                } else
                {
                    Reject(JSException.ErrorFrom(t.Exception));
                }
            });
        }

        internal JSPromise(JSValue value, PromiseState state) :
            base(JSContext.Current.PromisePrototype)
        {
            if (state == PromiseState.Pending)
            {
                RegisterPromise();
            }
            this.result = value;
            this.state = state;
        }


        public JSPromise(JSValue @delegate) :
            base(JSContext.Current.PromisePrototype)
        {
            // to improve speed of promise, we will add then/catch here...
            var sc = SynchronizationContext.Current;
            if (sc == null)
                throw JSContext.Current.NewTypeError($"Cannot use promise without Synchronization Context");

            RegisterPromise();

            resolveFunction = new JSFunction((in Arguments a) => Resolve(a.Get1()));
            rejectFunction = new JSFunction((in Arguments a) => Reject(a.Get1()));
            @delegate.InvokeFunction(new Arguments(this, resolveFunction, rejectFunction));

        }

        public JSPromise(JSPromiseDelegate @delegate) :
            base(JSContext.Current.PromisePrototype)
        {
            RegisterPromise();
            @delegate(p => Resolve(p), p => Reject(p));
        }

        /// <summary>
        /// This prevents garbage collection
        /// </summary>
        public JSPromise Parent { get; }
        public JSPromise(JSPromiseDelegate @delegate, JSPromise parent) :
            base(JSContext.Current.PromisePrototype)
        {
            this.Parent = parent;
            RegisterPromise();
            @delegate(p => Resolve(p), p => Reject(p));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSValue Resolve(JSValue value)
        {
            if (value == this)
            {
                Reject(JSContext.Current.NewTypeError("A promise cannot be resolved with itself").Error);
                return JSUndefined.Value;
            }

            pending.TryRemove(promiseID, out var __);

            // get then...
            if (value.IsObject)
            {
                var then = value[KeyStrings.then];
                if (then.IsFunction)
                {
                    // do what....
                    Post(() =>
                    {
                        try
                        {
                            then.InvokeFunction(new Arguments(value, new JSFunction((in Arguments a) => {
                                return Resolve(a.Get1());
                            }), new JSFunction((in Arguments a) => {
                                return Reject(a.Get1());
                            })));
                        }
                        catch (JSException jse)
                        {
                            Reject(jse.Error);
                        }
                        catch (Exception ex)
                        {
                            Reject(new JSString(ex.ToString()));
                        }
                    });
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
                    Post(t);
                }
            }

            return JSUndefined.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSValue Reject(JSValue value)
        {
            this.state = PromiseState.Rejected;
            this.result = value;

            pending.TryRemove(promiseID, out var __);

            var rejectList = this.rejectList;
            if (rejectList != null)
            {
                this.rejectList = null;
                foreach (var t in rejectList)
                {
                    Post(t);
                }
            }
            return JSUndefined.Value;
        }

        private TaskCompletionSource<JSValue> taskCompletion = null;
        public Task<JSValue> Task
        {
            get
            {
                if (taskCompletion == null)
                {
                    taskCompletion = new TaskCompletionSource<JSValue>();
                    this.thenList = this.thenList ?? new List<Action>();
                    thenList.Add(() => {
                        taskCompletion.TrySetResult(this.result);
                    });
                    this.rejectList = this.rejectList ?? new List<Action>();
                    rejectList.Add(() => {
                        taskCompletion.TrySetException(JSException.FromValue(this.result));
                    });
                }
                return taskCompletion.Task;
            }
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (type == typeof(Task<JSValue>) || type == typeof(Task))
            {
                value = Task;
                return true;
            }
            if (type.IsConstructedGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    value = JSPromiseExtensions.ToTaskInternal(this, type);
                    return true;
                }
            }
            return base.ConvertTo(type, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSValue Then(JSFunctionDelegate resolved, JSFunctionDelegate failed)
        {
            Action resolveAction = () =>
            {
                this.result = (resolved?.Invoke(new Arguments(this, this.result))) ?? this.result;
            };

            Action failAction = () => {
                this.result = (failed?.Invoke(new Arguments(this, this.result))) ?? this.result;
            };

            if (this.state == PromiseState.Resolved)
            {
                Post(resolveAction);
                return this;
            }
            if (this.state == PromiseState.Rejected)
            {
                Post(failAction);
                return this;
            }
            return new JSPromise((rs, rf) => {
                if (resolved != null)
                {
                    var thenList = this.thenList ?? (this.thenList = new List<Action>());
                    thenList.Add(() => {
                        try
                        {
                            resolveAction();
                            rs(this.result);
                        }
                        catch (Exception ex)
                        {
                            var error = JSError.From(ex);
                            rf(error);
                        }
                    });
                }
                if (failed != null)
                {
                    var catchList = this.rejectList ?? (this.rejectList = new List<Action>());
                    catchList.Add(() => {
                        try
                        {
                            failAction();
                            rf(this.resolveFunction);
                        } catch (Exception ex)
                        {
                            var error = JSError.From(ex);
                            rf(error);
                        }
                    });

                }
            }, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Post(Action action)
        {
            SynchronizationContext.Current.Post((_) => action(), null);
            //AsyncPump.Run(() => {
            //    action();
            //    return Task.CompletedTask;
            //});
        }
    }
}
