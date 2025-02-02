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
using YantraJS.Core.LightWeight;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.CodeGen;
using System.ComponentModel;
using YantraJS.Core.Core.DataView;
using YantraJS.Debugger;
using YantraJS.Core.Clr;
using YantraJS.Core.Core;
using YantraJS.Emit;

namespace YantraJS.Core
{

    public delegate JSValue JSClosureFunctionDelegate(ScriptInfo script, JSVariable[] closures, in Arguments a);

    public delegate JSValue JSFunctionDelegate(in Arguments a);

    public delegate void ConsoleEvent(JSContext context, string type, in Arguments a);

    public delegate void LogEventHandler(JSContext context, JSValue value);

    public delegate void ErrorEventHandler(JSContext context, Exception error);

    public class EvalEventArgs: EventArgs
    {
        public JSContext Context { get; set; }

        public string Script { get; set; }

        public string Location { get; set; }
    }

    public partial class JSContext: JSObject, IDisposable
    {

        private static long contextId = 1;

        public long ID { get; set; } = Interlocked.Increment(ref contextId);

        [ThreadStatic]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSContext Current;

        public JSDebugger Debugger;

        /// <summary>
        /// Available only when Enable Clr Integration is true in JSModuleContext
        /// </summary>
        public ClrMemberNamingConvention ClrMemberNamingConvention { get; set; }
            = ClrMemberNamingConvention.CamelCase;

        public static JSContext CurrentContext
        {
            get => Current;
            set {
                _current.Value = value;
                Current = value;
            }
        }

        private static readonly AsyncLocal<JSContext> _current = new AsyncLocal<JSContext>((e) => {
            Current = e.CurrentValue ?? e.PreviousValue;
        });

        // internal LinkedStack<LexicalScope> Stack = new LinkedStack<LexicalScope>();

        // internal LightWeightStack<CallStackItem> Stack = new LightWeightStack<CallStackItem>(256);

        // internal LinkedList<Task> waitTasks = new LinkedList<Task>();
        private TaskCompletionSource<int> _waitTask;
        internal Task WaitTask
        {
            get
            {
                return _waitTask?.Task;
            }
        }

        internal CallStackItem Top;

        public static JSFunction NewTarget
        {
            get
            {
                return Current.Top.NewTarget;
            }
        }

        public static JSObject NewTargetPrototype
        {
            get
            {
                return Current.Top?.NewTarget?.prototype;
            }
        }

        internal JSFunction CurrentNewTarget;


        public event EventHandler<EvalEventArgs> EvalEvent;

        internal void DispatchEvalEvent(ref string script, ref string location)
        {
            var ee = EvalEvent;
            if (ee != null)
            {
                var e = new EvalEventArgs { Context = this, Script = script, Location = location };
                EvalEvent.Invoke(this, e);
                script = e.Script;
                location = e.Location;
            }
        }

        public void Dispose()
        {
            _current.Value = null;
        }

        // public readonly JSObject StringPrototype;
        public readonly JSObject FunctionPrototype;

        // public readonly JSObject NumberPrototype;

        public new readonly JSObject ObjectPrototype;

        // public readonly JSObject ArrayPrototype;

        // public readonly JSObject BooleanPrototype;

        //public readonly JSObject TypeErrorPrototype;

        //public readonly JSObject EvalErrorPrototype;

        //public readonly JSObject ErrorPrototype;

        //public readonly JSObject RangeErrorPrototype;

        //public readonly JSObject SyntaxErrorPrototype;

        //public readonly JSObject URIErrorPrototype;

        //public readonly JSObject ReferenceErrorPrototype;

        // public readonly JSObject DatePrototype;

        // public readonly JSObject MapPrototype;

        // public readonly JSObject SetPrototype;

        // public readonly JSObject PromisePrototype;

        // public readonly JSObject RegExpPrototype;

        // public readonly JSObject WeakRefPrototype;

        // internal readonly JSObject WeakMapPrototype;

        // internal readonly JSObject WeakSetPrototype;

        // internal readonly JSObject GeneratorPrototype;

        // internal readonly JSObject BigIntPrototype;

        // public readonly JSObject ArrayBufferPrototype;

        //public readonly JSObject Int8ArrayPrototype;

        //public readonly JSObject Uint8ArrayPrototype;

        //public readonly JSObject Uint8ClampedArrayPrototype;

        //public readonly JSObject Int16ArrayPrototype;

        //public readonly JSObject Uint16ArrayPrototype;

        //public readonly JSObject Int32ArrayPrototype;

        //public readonly JSObject Uint32ArrayPrototype;

        //public readonly JSObject Float32ArrayPrototype;

        //public readonly JSObject Float64ArrayPrototype;

        // public readonly JSObject DataViewPrototype;

        // public readonly JSObject FinalizationRegistryPrototype;

        // public readonly JSObject JSON;

        // public readonly JSMath Math;

        public readonly JSFunction Object;

        // public readonly JSReflect Reflect;

        //public static JSContext Current
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    get
        //    {
        //        return _current.Value;
        //    }
        //    set
        //    {
        //        _current.Value = value;
        //        CurrentContext = value;
        //    }
        //}

        public event LogEventHandler Log;

        public event ErrorEventHandler Error;

        public event ConsoleEvent ConsoleEvent;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal int Push(string fileName, in StringSpan function, int line, int column)
        //{
        //    ref var top = ref Stack.Push(out var item);
        //    top.Function = function;
        //    top.FileName = fileName;
        //    top.Line = line;
        //    top.Column = column;
        //    return item;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal void Pop()
        //{
        //    Stack.Pop();
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal void Update(int index, int line, int column)
        //{
        //    ref var top = ref Stack.GetAt(index);
        //    top.Line = line;
        //    top.Column = column;
        //}

        //internal bool IsRootScope
        //{
        //    get
        //    {
        //        if (Stack.Count > 0)
        //        {
        //            ref var top = ref Stack.Top;
        //            return top.IsRootScope;
        //        } 
        //        return false;
        //    }
        //}

        SAUint32Map<JSVariable> globalVars = new SAUint32Map<JSVariable>();

        internal JSValue Register(JSVariable variable)
        {
            var v = variable.Value;
            var oldV = this[variable.Name];
            if (oldV != v)
            {
                // avoid IsReadOnly error
                this[variable.Name] = v;
            }
            KeyString name = variable.Name;
            globalVars.Put(name.Key) = variable;
            return v;
        }

        public override JSValue this[KeyString name] { 
            get => base[name];
            set
            {
                base[name] = value;
                if(globalVars.TryGetValue(name.Key, out var jsv))
                {
                    jsv.Value = value;
                }
            }
        }

        //internal LightWeightStack<CallStackItem>.StackWalker StackWalker
        //{
        //    get
        //    {
        //        return Stack.Walker;
        //    }
        //}

        internal void FillStackTrace(StringBuilder sb)
        {
        }

        //internal LightWeightStack<CallStackItem> CloneStack()
        //{
        //    var copy = new LightWeightStack<CallStackItem>(this.Stack);
        //    return copy;
        //}

        //internal LightWeightStack<CallStackItem> Switch(LightWeightStack<CallStackItem> newValue)
        //{
        //    var old = this.Stack;
        //    this.Stack = newValue;
        //    return old;
        //}

        public JSContext(SynchronizationContext synchronizationContext = null)
        {
            this.synchronizationContext = synchronizationContext ?? SynchronizationContext.Current;

            // Scope.Push(new LexicalScope("", "", 1, 1));
            // Scope.Top.IsRoot = true;
            Current = this;
            _current.Value = this;

            ref var ownProperties = ref this.GetOwnProperties();

            //T CreateInternalObject<T>(KeyString name)
            //    where T: JSObject
            //{
            //    var r = Activator.CreateInstance<T>();
            //    ref var rop = ref r.GetOwnProperties();
            //    var cached = cache.GetOrCreate(name.Key, () => { 
            //        return Bootstrap.Create(name, typeof(T));
            //    });

            //    ref var op = ref this.GetOwnProperties();

            //    op.Put(name, r, JSPropertyAttributes.ConfigurableReadonlyValue);
            //    var ve = cached.GetOwnProperties().GetEnumerator(false);
            //    while(ve.MoveNext(out var keyString, out var value))
            //    {
            //        rop.Put(keyString.Key) = value;
            //    }

            //    return r;
            //}

            this[Names.Symbol] = JSSymbol.CreateClass(this, false);
            var func = JSFunction.CreateClass(this, false);
            this[Names.Function] = func;
            FunctionPrototype = func.prototype;
            Object = JSObject.CreateClass(this, false);
            this[Names.Object] = Object;
            ObjectPrototype = Object.prototype;
            ObjectPrototype.BasePrototypeObject = null;
            // ObjectPrototype.Delete(KeyStrings.constructor);
            func.BasePrototypeObject = Object;
            // Object.BasePrototypeObject = null;
            FunctionPrototype.BasePrototypeObject = ObjectPrototype;

            // create object prototype...
            // Object =  this.Create<JSObject>(KeyStrings.Object);
            //Object.f = JSObjectPrototype.Constructor;
            //ObjectPrototype.Delete(KeyStrings.constructor);
            // ObjectPrototype.BasePrototypeObject = null;
            // func.BasePrototypeObject = Object;
            // FunctionPrototype.BasePrototypeObject = ObjectPrototype;
            // ArrayPrototype = this.Create<JSArray>(KeyStrings.Array).prototype;
            // StringPrototype = this.Create<JSString>(KeyStrings.String).prototype;
            // NumberPrototype = this.Create<JSNumber>(KeyStrings.Number).prototype;
            // BooleanPrototype = this.Create<JSBoolean>(KeyStrings.Boolean).prototype;
            //ErrorPrototype = this.Create<JSError>(KeyStrings.Error).prototype;
            //EvalErrorPrototype = this.Create<JSError>(KeyStrings.EvalError, ErrorPrototype).prototype;
            //TypeErrorPrototype = this.Create<JSError>(KeyStrings.TypeError, ErrorPrototype).prototype;
            //RangeErrorPrototype = this.Create<JSError>(KeyStrings.RangeError, ErrorPrototype).prototype;
            //SyntaxErrorPrototype = this.Create<JSError>(KeyStrings.SyntaxError, ErrorPrototype).prototype;
            //URIErrorPrototype = this.Create<JSError>(KeyStrings.URIError, ErrorPrototype).prototype;
            //ReferenceErrorPrototype = this.Create<JSError>(KeyStrings.ReferenceError, ErrorPrototype).prototype;
            // DatePrototype = this.Create<JSDate>(KeyStrings.Date).prototype;
            // MapPrototype = this.Create<JSMap>(KeyStrings.Map).prototype;
            // PromisePrototype = this.Create<JSPromise>(KeyStrings.Promise).prototype;
            // RegExpPrototype = this.Create<JSRegExp>(KeyStrings.RegExp).prototype;
            // SetPrototype = this.Create<JSSet>(KeyStrings.Set).prototype;
            // WeakRefPrototype = this.Create<JSWeakRef>(KeyStrings.WeakRef).prototype;
            // WeakSetPrototype = this.Create<JSWeakSet>(KeyStrings.WeakSet).prototype;
            // WeakMapPrototype = this.Create<JSWeakMap>(KeyStrings.WeakMap).prototype;
            // GeneratorPrototype = this.Create<JSGenerator>(KeyStrings.Generator).prototype;
            // BigIntPrototype = this.Create<JSBigInt>(KeyStrings.BigInt).prototype;
            // ArrayBufferPrototype = this.Create<JSArrayBuffer>(KeyStrings.ArrayBuffer).prototype;
            //Int8ArrayPrototype = this.Create<Int8Array>(KeyStrings.Int8Array).prototype;
            //Uint8ArrayPrototype = this.Create<Uint8Array>(KeyStrings.Uint8Array).prototype;
            //Uint8ClampedArrayPrototype = this.Create<Uint8ClampedArray>(KeyStrings.Uint8ClampedArray).prototype;
            //Int16ArrayPrototype = this.Create<Int16Array>(KeyStrings.Int16Array).prototype;
            //Uint16ArrayPrototype = this.Create<Uint16Array>(KeyStrings.Uint16Array).prototype;
            //Int32ArrayPrototype = this.Create<Int32Array>(KeyStrings.Int32Array).prototype;
            //Uint32ArrayPrototype = this.Create<Uint32Array>(KeyStrings.Uint32Array).prototype;
            //Float32ArrayPrototype = this.Create<Float32Array>(KeyStrings.Float32Array).prototype;
            //Float64ArrayPrototype = this.Create<Float64Array>(KeyStrings.Float64Array).prototype;
            // DataViewPrototype = this.Create<DataView>(KeyStrings.DataView).prototype;
            // FinalizationRegistryPrototype = this.Create<JSFinalizationRegistry>(KeyStrings.FinalizationRegistry).prototype;
            // JSON = CreateInternalObject<JSJSON>(KeyStrings.JSON);
            // Math = CreateInternalObject<JSMath>(KeyStrings.Math);
            // Reflect = CreateInternalObject<JSReflect>(KeyStrings.Reflect);

            this.RegisterGeneratedClasses();
            // this.Fill<JSGlobalStatic>();

            //var c = new JSObject
            //{
            //    BasePrototypeObject = (Bootstrap.Create("console", typeof(JSConsole))).prototype
            //};
            this[KeyStrings.console] = Clr.ClrProxy.From(new JSConsole(this));

            this[KeyStrings.debug] = new JSFunction(this.Debug);

        }

        internal void FireConsoleEvent(string type, in Arguments a)
        {
            ConsoleEvent?.Invoke(this, type, in a);
        }

        private JSValue Debug(in Arguments a)
        {
            System.Diagnostics.Debug.WriteLine(a.Get1().ToString());
            return JSUndefined.Value;
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


        static readonly ConcurrentUInt32Map<JSFunction> cache = ConcurrentUInt32Map<JSFunction>.Create();
        internal readonly SynchronizationContext synchronizationContext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewTypeError(string message, 
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return (new JSTypeError(new Arguments(JSUndefined.Value, new JSString(message)), function: function, filePath: filePath, line: line)).Exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewSyntaxError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return (new JSSyntaxError(new Arguments(JSUndefined.Value, new JSString(message)), function: function, filePath: filePath, line: line)).Exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewURIError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return (new JSURIError(new Arguments(JSUndefined.Value, new JSString(message)), function: function, filePath: filePath, line: line)).Exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewRangeError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return (new JSRangeError(new Arguments(JSUndefined.Value, new JSString(message)), function: function, filePath: filePath, line: line)).Exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JSException NewError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return (new JSError(new Arguments(JSUndefined.Value, new JSString(message)), function: function, filePath: filePath, line: line)).Exception;
        }

        partial void OnError(Exception ex);

        internal void ReportError(Exception ex)
        {
            OnError(ex);
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

        private static long nextTimeout = 1;
        private static long nextInterval = 1;

        internal long PostTimeout(int delay, JSFunction f, in Arguments a)
        {
            var ctx = this.synchronizationContext;
            if(ctx == null)
            {
                throw NewTypeError($"Synchronization context must be present to set timeout");
            }
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
            var timer = new Timer((_) => {
                ctx.Post((x) => {
                    var f = x as JSValue;
                    try
                    {
                        f.InvokeFunction(new Arguments(JSUndefined.Value, args));
                    }catch (Exception ex)
                    {
                        this.ReportError(ex);
                    }
                    ClearTimeout(key);
                }, f);
            }, f, delay, Timeout.Infinite);

            timeouts.AddOrUpdate(key, timer, (a1, a2) => a2);
            lock(this)
            {
                _waitTask = _waitTask ?? new TaskCompletionSource<int>();
            }
            return key;
        }
        internal long SetInterval(int delay, JSFunction f, in Arguments a)
        {
            var ctx = this.synchronizationContext;
            if (ctx == null)
            {
                throw NewTypeError($"Synchronization context must be present to set timeout");
            }
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
            var timer = new Timer((_) => {
                ctx.Post(f, (x) => {
                    try
                    {
                        x.InvokeFunction(new Arguments(JSUndefined.Value, args));
                    }catch (Exception ex)
                    {
                        this.ReportError(ex);
                    }
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

        public ICodeCache CodeCache = DictionaryCodeCache.Current;

        internal ConcurrentDictionary<long, JSPromise> PendingPromises
            = new ConcurrentDictionary<long, JSPromise>();

        /// <summary>
        /// Quickly evaluates the code, does not wait for promises and timeouts/intervals.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeFilePath"></param>
        /// <returns></returns>
        public JSValue Eval(string code, string codeFilePath = null, JSValue @this = null)
        {
            @this ??= this;
            if (Debugger == null)
            {
                var fx = CoreScript.Compile(code, codeFilePath, codeCache: CodeCache);
                return fx(new Arguments(@this));
            }

            try
            {
                var f = CoreScript.Compile(code, codeFilePath, codeCache: CodeCache);
                Debugger.ScriptParsed(this.ID, code, codeFilePath);
                return f(new Arguments(@this));
            }
            catch (Exception ex) {
                this.ReportError(ex);
                throw ex;
            }
            // return CoreScript.Evaluate(code, codeFilePath);
        }

        /// <summary>
        /// Evaluates the given code, waits for the promise and returns task that
        /// completes till all timeouts/intervals are completed.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeFilePath"></param>
        /// <returns></returns>
        public async Task<JSValue> ExecuteAsync(string code, string codeFilePath = null)
        {
            var r = CoreScript.Evaluate(code, codeFilePath, codeCache: CodeCache);
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
        }


        /// <summary>
        /// Evaluates the given code, waits for the promise and also 
        /// waits synchronously (by running and AsyncPump) for timeouts/intervals to finish
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeFilePath"></param>
        /// <returns></returns>
        public JSValue Execute(string code, string codeFilePath = null)
        {
            return AsyncPump.Run(() => ExecuteAsync(code, codeFilePath));
        }

    }
}
