using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Core.Typed;
using YantraJS.Core.BigInt;
using YantraJS.Core.Debug;
using YantraJS.Core.Generator;
using YantraJS.Core.Objects;
using YantraJS.Core.Set;
using YantraJS.Core.Weak;
using System.Collections.Concurrent;
using Microsoft.Threading;

namespace YantraJS.Core
{

    public delegate JSValue JSFunctionDelegate(in Arguments a);

    public delegate void LogEventHandler(JSContext context, JSValue value);

    public delegate void ErrorEventHandler(JSContext context, Exception error);

    public class JSContext: JSObject, IDisposable
    {

        static readonly AsyncLocal<JSContext> _current = new AsyncLocal<JSContext>();

        internal LinkedStack<LexicalScope> Scope = new LinkedStack<LexicalScope>();

        // internal LinkedList<Task> waitTasks = new LinkedList<Task>();
        private TaskCompletionSource<int> _waitTask;
        internal Task WaitTask
        {
            get
            {
                return _waitTask?.Task;
            }
        }

        public void Dispose()
        {
            _current.Value = null;
        }

        public readonly JSObject StringPrototype;
        public readonly JSObject FunctionPrototype;

        public readonly JSObject NumberPrototype;

        public readonly JSObject ObjectPrototype;

        public readonly JSObject ArrayPrototype;

        public readonly JSObject BooleanPrototype;

        public readonly JSObject TypeErrorPrototype;

        public readonly JSObject ErrorPrototype;

        public readonly JSObject RangeErrorPrototype;

        public readonly JSObject SyntaxErrorPrototype;

        public readonly JSObject URIErrorPrototype;

        public readonly JSObject DatePrototype;

        public readonly JSObject MapPrototype;

        public readonly JSObject SetPrototype;

        public readonly JSObject PromisePrototype;

        public readonly JSObject RegExpPrototype;

        public readonly JSObject WeakRefPrototype;

        internal readonly JSObject WeakMapPrototype;

        internal readonly JSObject WeakSetPrototype;

        internal readonly JSObject GeneratorPrototype;

        internal readonly JSObject BigIntPrototype;

        public readonly JSObject ArrayBufferPrototype;

        public readonly JSObject Int8ArrayPrototype;

        public readonly JSObject Uint8ArrayPrototype;

        public readonly JSObject Int16ArrayPrototype;

        public readonly JSObject Uint16ArrayPrototype;

        public readonly JSObject Int32ArrayPrototype;

        public readonly JSObject Uint32ArrayPrototype;

        public readonly JSObject Float32ArrayPrototype;

        public readonly JSObject Float64ArrayPrototype;

        public readonly JSObject JSON;

        public readonly JSMath Math;

        public readonly JSFunction Object;

        public static JSContext Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _current.Value;
            }
            set
            {
                _current.Value = value;
            }
        }

        public event LogEventHandler Log;

        public event ErrorEventHandler Error;

        public JSContext(SynchronizationContext synchronizationContext = null)
        {
            this.synchronizationContext = synchronizationContext ?? SynchronizationContext.Current;

            Scope.Push(new LexicalScope("", "", 1, 1));
            Scope.Top.IsRoot = true;

            _current.Value = this;

            ref var ownProperties = ref this.GetOwnProperties();

            T CreateInternalObject<T>(KeyString name)
                where T: JSObject
            {
                var r = Activator.CreateInstance<T>();
                ref var rop = ref r.GetOwnProperties();
                var cached = cache.GetOrCreate(name.Key, () => { 
                    return Bootstrap.Create(name, typeof(T));
                });

                ref var op = ref this.GetOwnProperties();

                op[name.Key] = JSProperty.Property(r, JSPropertyAttributes.ConfigurableReadonlyValue);

                foreach(var (Key, Value) in cached.GetOwnProperties().AllValues())
                {
                    rop[Key] = Value;
                }

                return r;
            }

            this.Create<JSSymbol>(KeyStrings.Symbol);
            var func = this.Create<JSFunction>(KeyStrings.Function);
            FunctionPrototype = func.prototype;
            // create object prototype...
            Object =  this.Create<JSObject>(KeyStrings.Object);
            ObjectPrototype = Object.prototype;
            func.prototypeChain = Object;
            FunctionPrototype.prototypeChain = ObjectPrototype;
            ArrayPrototype = this.Create<JSArray>(KeyStrings.Array).prototype;
            StringPrototype = this.Create<JSString>(KeyStrings.String).prototype;
            NumberPrototype = this.Create<JSNumber>(KeyStrings.Number).prototype;
            BooleanPrototype = this.Create<JSBoolean>(KeyStrings.Boolean).prototype;
            ErrorPrototype = this.Create<JSError>(KeyStrings.Error).prototype;
            TypeErrorPrototype = this.Create<JSError>(KeyStrings.TypeError, ErrorPrototype).prototype;
            RangeErrorPrototype = this.Create<JSError>(KeyStrings.RangeError, ErrorPrototype).prototype;
            SyntaxErrorPrototype = this.Create<JSError>(KeyStrings.SyntaxError, ErrorPrototype).prototype;
            URIErrorPrototype = this.Create<JSError>(KeyStrings.URIError, ErrorPrototype).prototype;
            DatePrototype = this.Create<JSDate>(KeyStrings.Date).prototype;
            MapPrototype = this.Create<JSMap>(KeyStrings.Map).prototype;
            PromisePrototype = this.Create<JSPromise>(KeyStrings.Promise).prototype;
            RegExpPrototype = this.Create<JSRegExp>(KeyStrings.RegExp).prototype;
            SetPrototype = this.Create<JSSet>(KeyStrings.Set).prototype;
            WeakRefPrototype = this.Create<JSWeakRef>(KeyStrings.WeakRef).prototype;
            WeakSetPrototype = this.Create<JSWeakSet>(KeyStrings.WeakSet).prototype;
            WeakMapPrototype = this.Create<JSWeakMap>(KeyStrings.WeakMap).prototype;
            GeneratorPrototype = this.Create<JSGenerator>(KeyStrings.Generator).prototype;
            BigIntPrototype = this.Create<JSBigInt>(KeyStrings.BigInt).prototype;
            ArrayBufferPrototype = this.Create<JSArrayBuffer>(KeyStrings.ArrayBuffer).prototype;
            Int8ArrayPrototype = this.Create<Int8Array>(KeyStrings.Int8Array).prototype;
            Uint8ArrayPrototype = this.Create<Uint8Array>(KeyStrings.Uint8Array).prototype;
            Int16ArrayPrototype = this.Create<Int16Array>(KeyStrings.Int16Array).prototype;
            Uint16ArrayPrototype = this.Create<Uint16Array>(KeyStrings.Uint16Array).prototype;
            Int32ArrayPrototype = this.Create<Int32Array>(KeyStrings.Int32Array).prototype;
            Uint32ArrayPrototype = this.Create<Uint32Array>(KeyStrings.Uint32Array).prototype;
            Float32ArrayPrototype = this.Create<Float32Array>(KeyStrings.Float32Array).prototype;
            Float64ArrayPrototype = this.Create<Float64Array>(KeyStrings.Float64Array).prototype;
            JSON = CreateInternalObject<JSJSON>(KeyStrings.JSON);
            Math = CreateInternalObject<JSMath>(KeyStrings.Math);

            this.Fill<JSGlobalStatic>();

            var c = new JSObject
            {
                prototypeChain = (Bootstrap.Create("console", typeof(JSConsole))).prototype
            };
            this[KeyStrings.console] = c;

        }

        internal readonly ConcurrentDictionary<long, Timer> timeouts = new ConcurrentDictionary<long, Timer>();
        internal readonly ConcurrentDictionary<long, Timer> timers = new ConcurrentDictionary<long, Timer>();

        internal void ClearTimeout(long n)
        {
            if(timeouts.TryRemove(n, out var timer))
            {
                try { timer.Dispose(); } catch { }
            }
            if (timers.Count == 0 && timeouts.Count == 0)
            {
                _waitTask.TrySetResult(1);
            }
        }

        internal void ClearInterval(long n)
        {
            if (timers.TryRemove(n, out var timer))
            {
                try { timer.Dispose(); } catch { }
            }
            if (timers.Count == 0 && timeouts.Count == 0)
            {
                _waitTask.TrySetResult(1);
            }
        }


        static readonly ConcurrentUInt32Trie<JSFunction> cache = new ConcurrentUInt32Trie<JSFunction>();
        internal readonly SynchronizationContext synchronizationContext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewTypeError(string message, 
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, TypeErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewSyntaxError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, SyntaxErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewURIError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, URIErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewRangeError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, RangeErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, ErrorPrototype, function, filePath, line);
        }

        internal void ReportError(Exception ex)
        {
            Error?.Invoke(this, ex);
            //var cx = this[KeyStrings.console];
            //if (cx.IsUndefined)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex);
            //    return;
            //}

            //var log = cx[KeyStrings.log];
            //if (log.IsUndefined)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex);
            //    return;
            //}
            //log.InvokeFunction(new Arguments(cx, new JSString(ex.ToString())));
        }
        public void ReportLog(JSValue f)
        {
            Log?.Invoke(this, f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        private void RunAction(Action action)
        {
            try
            {
                action();
            } catch (Exception ex)
            {
                this.ReportError(ex);
            }
        }

        internal void Post(SynchronizationContext ctx, Action action)
        {
            if (ctx != null)
            {
                ctx.Post((_) => RunAction(action), null);
                return;
            }
            RunAction(action);
        }

        public void Post(Action action)
        {
            var ctx = SynchronizationContext.Current;
            if (ctx != null)
            {
                ctx.Post((_) => RunAction(action), null);
                return;
            }
            RunAction(action);
        }

        private static long nextTimeout = 1;
        private static long nextInterval = 1;

        internal long PostTimeout(int delay, JSFunction f, in Arguments a)
        {
            var key = Interlocked.Increment(ref nextTimeout);
            JSValue[] args = JSArguments.Empty;
            if (a.Length > 2)
            {
                args = new JSValue[a.Length - 2];
                for (int i = 2; i < a.Length; i++)
                {
                    args[i - 2] = a.GetAt(i);
                }
            }
            var ctx = SynchronizationContext.Current;
            var timer = new Timer((_) => {
                Post(ctx, () => {
                    f.InvokeFunction(new Arguments(JSUndefined.Value, args));
                    ClearTimeout(key);
                });
            }, f, delay, Timeout.Infinite);

            timeouts.AddOrUpdate(key, timer, (a1, a2) => a2);
            lock(this)
            {
                _waitTask = _waitTask ?? new TaskCompletionSource<int>();
            }
            return key;
        }
        internal long SetInterval(int delay, JSFunction f, Arguments a)
        {
            var key = Interlocked.Increment(ref nextInterval);
            JSValue[] args = JSArguments.Empty;
            if (a.Length > 2)
            {
                args = new JSValue[a.Length - 2];
                for (int i = 2; i < a.Length; i++)
                {
                    args[i - 2] = a.GetAt(i);
                }
            }
            var ctx = SynchronizationContext.Current;
            var timer = new Timer((_) => {
                Post(ctx, () => {
                    f.InvokeFunction(new Arguments(JSUndefined.Value, args));
                    ClearInterval(key);
                });
            }, f, delay, Timeout.Infinite);

            timers.AddOrUpdate(key, timer, (a1, a2) => a2);
            lock (this)
            {
                _waitTask = _waitTask ?? new TaskCompletionSource<int>();
            }
            return key;

        }


        internal ConcurrentDictionary<long, JSPromise> PendingPromises
            = new ConcurrentDictionary<long, JSPromise>();

        /// <summary>
        /// Quickly evaluate the code, does not wait for promises and timeouts/intervals
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeFilePath"></param>
        /// <returns></returns>
        public JSValue FastEval(string code, string codeFilePath = null)
        {
            return CoreScript.Evaluate(code, codeFilePath);
        }


        /// <summary>
        /// Evaluates the given code, waits for the promise and also 
        /// waits for timeouts/intervals to finish
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeFilePath"></param>
        /// <returns></returns>
        public JSValue Eval(string code, string codeFilePath = null)
        {
            return AsyncPump.Run<JSValue>(async () => {
                var r = CoreScript.Evaluate(code, codeFilePath);
                var wt = this.WaitTask;
                if (wt != null)
                    await wt;
                if (r is JSPromise promise)
                {
                    return await promise.Task;
                }
                if (r is JSObject @object)
                {
                    var then = @object[KeyStrings.then];
                    if (then.IsFunction)
                    {
                        promise = new JSPromise((resolve, reject) => {
                            var resolveF = new JSFunction((in Arguments a) => {
                                var a1 = a.Get1();
                                resolve(a1);
                                return a1;
                            });
                            var rejectF = new JSFunction((in Arguments a) => {
                                var a1 = a.Get1();
                                reject(a1);
                                return a1;
                            });
                            var a = new Arguments(@object, resolveF, rejectF);
                            then.InvokeFunction(a);
                        });
                        return await promise.Task;
                    }
                }
                return r;
            });
        }


    }
}
