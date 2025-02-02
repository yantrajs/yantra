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
using Yantra.Core;
using YantraJS.Core.Clr;
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
    // [JSRuntime(typeof(JSPromiseStatic), typeof(JSPromisePrototype))]
    [JSFunctionGenerator("Promise")]
    public partial class JSPromise : JSObject
    {


        internal enum PromiseState
        {
            Pending,
            Resolved,
            Rejected
        }

        private enum ReactionType
        {
            Resolve,
            Reject
        }

        private class Reaction
        {
            public JSPromise Promise;
            public  ReactionType Type;
            public JSFunctionDelegate Handler;
        }

        internal PromiseState state = PromiseState.Pending;

        private Sequence<Reaction> thenList;
        private Sequence<Reaction> rejectList;
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

        internal JSPromise(JSValue value, PromiseState state): this()
        {
            InitPromise();
            this.state = state;
            this.result = value;
        }

        /// <summary>
        /// Promise must stay alive till resolved...
        /// </summary>
        /// <param name="value"></param>
        public JSPromise(Task<JSValue> value) : this()
        {
            sc = JSContext.Current.synchronizationContext;
            RegisterPromise();
            value.ContinueWith((t) => {
                if (t.IsCompleted)
                {
                    if (t.IsFaulted)
                    {
                        Reject(JSException.ErrorFrom(t.Exception));
                        return;
                    }
                    Resolve(t.Result);
                } else
                {
                    Reject(JSException.ErrorFrom(t.Exception));
                }
            });
        }

        public JSPromise(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            InitPromise();
            JSValue @delegate = a[0];
            try
            {
                @delegate.InvokeFunction(new Arguments(this, resolveFunction, rejectFunction));
            }
            catch (Exception ex)
            {
                rejectFunction.InvokeFunction(new Arguments(JSUndefined.Value, JSError.From(ex)));
            }
        }

        //public JSPromise(JSValue @delegate): this()
        //{
        //    InitPromise();
        //    try
        //    {
        //        @delegate.InvokeFunction(new Arguments(this, resolveFunction, rejectFunction));
        //    }
        //    catch (Exception ex)
        //    {
        //        rejectFunction.InvokeFunction(new Arguments(JSUndefined.Value, JSError.From(ex)));
        //    }
        //}

        public JSPromise(JSPromiseDelegate @delegate) : this()
        {
            InitPromise();
            try
            {
                @delegate((v) => resolveFunction.Call(JSUndefined.Value,v), (v) => rejectFunction.Call(JSUndefined.Value, v));
            }
            catch (Exception ex)
            {
                rejectFunction.InvokeFunction(new Arguments(JSUndefined.Value, JSError.From(ex)));
            }
        }

        private void InitPromise()
        {
            // to improve speed of promise, we will add then/catch here...
            sc = JSContext.Current.synchronizationContext;
            if (sc == null)
                throw JSContext.Current.NewTypeError($"Cannot use promise without Synchronization Context");

            RegisterPromise();

            resolveFunction = new JSFunction((in Arguments a) =>
            {
                Resolve(a.Get1());
                return JSUndefined.Value;
            });
            rejectFunction = new JSFunction((in Arguments a) =>
            {
                Reject(a.Get1());
                return JSUndefined.Value;
            });

        }


        /// <summary>
        /// This prevents garbage collection
        /// </summary>
        public JSPromise Parent { get; }
        //public JSPromise(JSPromiseDelegate @delegate, JSPromise parent) :
        //    base(JSContext.Current.PromisePrototype)
        //{
        //    sc = JSContext.Current.synchronizationContext;
        //    this.Parent = parent;
        //    RegisterPromise();
        //    @delegate(p => Resolve(p), p => Reject(p));
        //}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Resolve(JSValue value)
        {
            if (state != PromiseState.Pending)
                return;

            if (value == this)
            {
                Reject(JSContext.Current.NewTypeError("A promise cannot be resolved with itself").Error);
                return;
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
                            then.Call(value, resolveFunction, rejectFunction);
                        }
                        catch (Exception ex)
                        {
                            Reject(JSError.From(ex));
                        }
                    });
                    return;
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

            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reject(JSValue value)
        {
            if (state != PromiseState.Pending)
                return;

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
            return;
        }

        private TaskCompletionSource<JSValue> taskCompletion = null;
        private SynchronizationContext sc;

        public Task<JSValue> Task
        {
            get
            {
                if(state == PromiseState.Resolved)
                {
                    return System.Threading.Tasks.Task.FromResult(result);
                }
                if (state == PromiseState.Rejected)
                {
                    throw JSException.FromValue(result);
                }
                if (taskCompletion == null)
                {
                    taskCompletion = new TaskCompletionSource<JSValue>();
                    this.Then((in Arguments a) => {
                        taskCompletion.TrySetResult(a.Get1());
                        return JSUndefined.Value;
                    },(in Arguments a) => {
                        taskCompletion.TrySetException(JSException.FromValue(result));
                        return JSUndefined.Value;
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
        internal JSValue Then(JSFunctionDelegate resolve, JSFunctionDelegate fail, JSPromise @return = null)
        {
            // @return ??= new JSPromise();
            if (@return == null)
            {
                @return = new JSPromise();
                @return.InitPromise();
            }
            var resolved = new Reaction { Promise = @return, Type = ReactionType.Resolve, Handler = resolve };
            var rejected = new Reaction { Promise = @return, Type = ReactionType.Reject, Handler = fail };

            if(state == PromiseState.Pending)
            {
                this.rejectList ??= new Sequence<Reaction>();
                this.thenList ??= new Sequence<Reaction>();
                rejectList.Add(rejected);
                thenList.Add(resolved);
            } else if(state == PromiseState.Resolved)
            {
                Post(resolved);
            } else
            {
                Post(rejected);
            }

            return @return;
        }

        private void Post(Reaction reaction)
        {
            Post(() => { 
                if(reaction.Handler != null)
                {
                    try {
                        var handlerResult = reaction.Handler(new Arguments(JSUndefined.Value, result));
                        if(reaction.Promise != null)
                        {
                            reaction.Promise.Resolve(handlerResult);
                        }
                    } catch(Exception ex) {
                        reaction.Promise.Reject(JSError.From(ex));
                    }
                } else if (reaction.Type == ReactionType.Resolve)
                {
                    if(reaction.Promise != null)
                    {
                        reaction.Promise.Resolve(this.result ?? JSUndefined.Value);
                    }
                } else
                {
                    if(reaction.Promise != null)
                    {
                        reaction.Promise.Reject(this.result ?? JSUndefined.Value);
                    }
                }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Post(Action action)
        {
            sc.Post(action, (x) => x());
            //AsyncPump.Run(() => {
            //    action();
            //    return Task.CompletedTask;
            //});
        }
    }
}
