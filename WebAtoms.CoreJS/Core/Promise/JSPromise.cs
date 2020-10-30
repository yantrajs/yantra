using Microsoft.Threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core.Clr;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Core.Storage;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
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

        /// <summary>
        /// .Net removes promises aggressively via
        /// garbage collection... so all promises
        /// till resolved/failed are stored in 
        /// global list
        /// </summary>
        private void  RegisterPromise()
        {
            //if (promiseID == 0)
            //{
            //    promiseID = Interlocked.Increment(ref nextPromiseID);

            //    var pending = JSContext.Current.PendingPromises;
            //    if (pending.TryAdd(promiseID, this))
            //    {
            //        JSFunctionDelegate done = (in Arguments a) =>
            //        {
            //            pending.TryRemove(promiseID, out var _);
            //            return JSUndefined.Value;
            //        };
            //        this.Then(done, done);
            //    }
            //}
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSValue Resolve(JSValue value)
        {
            if (value == this)
            {
                Reject(JSContext.Current.NewTypeError("A promise cannot be resolved with itself").Error);
                return JSUndefined.Value;
            }

            // get then...
            if (value.IsObject)
            {
                var then = value["then"];
                if (then.IsFunction)
                {
                    // do what....
                    Post(() =>
                    {
                        try
                        {
                            then.InvokeFunction(new Arguments(value, resolveFunction, rejectFunction));
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
                        Post(() => {
                            taskCompletion.TrySetResult(this.result);
                        });
                    });
                    rejectList = rejectList ?? new List<Action>();
                    rejectList.Add(() => {
                        Post(() => {
                            taskCompletion.TrySetException(JSException.FromValue(this.result));
                        });
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
            });
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
