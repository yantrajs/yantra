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
using WebAtoms.CoreJS.Core.BigInt;
using WebAtoms.CoreJS.Core.Generator;
using WebAtoms.CoreJS.Core.Objects;
using WebAtoms.CoreJS.Core.Set;
using WebAtoms.CoreJS.Core.Weak;

namespace WebAtoms.CoreJS.Core
{

    public delegate JSValue JSFunctionDelegate(in Arguments a);

    public class JSContext: JSObject, IDisposable
    {

        static AsyncLocal<JSContext> _current = new AsyncLocal<JSContext>();

        internal LinkedStack<LexicalScope> Scope = new LinkedStack<LexicalScope>();        

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

        public readonly JSObject JSON;

        public readonly JSMath Math;

        public static JSContext Current
        {
            get
            {
                return _current.Value;
            }
            set
            {
                _current.Value = value;
            }
        }

        public JSContext(SynchronizationContext synchronizationContext = null)
        {
            this.synchronizationContext = synchronizationContext ?? SynchronizationContext.Current;

            Scope.Push(new LexicalScope("", "", 1, 1));
            Scope.Top.IsRoot = true;

            _current.Value = this;

            ownProperties = new PropertySequence();

            T CreateInternalObject<T>(KeyString name)
                where T: JSObject
            {
                var r = Activator.CreateInstance<T>();
                r.ownProperties = new PropertySequence();
                var cached = cache.GetOrCreate(name.Key, () => { 
                    return Bootstrap.Create(name, typeof(T));
                });

                ownProperties[name.Key] = JSProperty.Property(r, JSPropertyAttributes.ConfigurableReadonlyValue);

                foreach(var p in cached.ownProperties.AllValues())
                {
                    r.ownProperties[p.Key] = p.Value;
                }

                return r;
            }

            this.Create<JSSymbol>(KeyStrings.Symbol);
            FunctionPrototype = this.Create<JSFunction>(KeyStrings.Function);
            // create object prototype...
            ObjectPrototype =  this.Create<JSObject>(KeyStrings.Object);
            ArrayPrototype = this.Create<JSArray>(KeyStrings.Array);
            StringPrototype = this.Create<JSString>(KeyStrings.String);
            NumberPrototype = this.Create<JSNumber>(KeyStrings.Number);
            BooleanPrototype = this.Create<JSBoolean>(KeyStrings.Boolean);
            ErrorPrototype = this.Create<JSError>(KeyStrings.Error);
            TypeErrorPrototype = this.Create<JSTypeError>(KeyStrings.TypeError, ErrorPrototype);
            RangeErrorPrototype = this.Create<JSTypeError>(KeyStrings.RangeError, ErrorPrototype);
            SyntaxErrorPrototype = this.Create<JSTypeError>(KeyStrings.SyntaxError, ErrorPrototype);
            URIErrorPrototype = this.Create<JSTypeError>(KeyStrings.URIError, ErrorPrototype);
            DatePrototype = this.Create<JSDate>(KeyStrings.Date);
            MapPrototype = this.Create<JSMap>(KeyStrings.Map);
            PromisePrototype = this.Create<JSPromise>(KeyStrings.Promise);
            RegExpPrototype = this.Create<JSRegExp>(KeyStrings.RegExp);
            SetPrototype = this.Create<JSSet>(KeyStrings.Set);
            WeakRefPrototype = this.Create<JSWeakRef>(KeyStrings.WeakRef);
            WeakSetPrototype = this.Create<JSWeakSet>(KeyStrings.WeakSet);
            WeakMapPrototype = this.Create<JSWeakMap>(KeyStrings.WeakMap);
            GeneratorPrototype = this.Create<JSGenerator>(KeyStrings.Generator);
            BigIntPrototype = this.Create<JSBigInt>(KeyStrings.BigInt);
            JSON = CreateInternalObject<JSJSON>(KeyStrings.JSON);
            Math = CreateInternalObject<JSMath>(KeyStrings.Math);

            this.Fill<JSGlobalStatic>();
        }

        private static ConcurrentUInt32Trie<JSFunction> cache = new ConcurrentUInt32Trie<JSFunction>();
        internal readonly SynchronizationContext synchronizationContext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewTypeError(string message, 
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, TypeErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewSyntaxError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, SyntaxErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewURIError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, URIErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewRangeError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, RangeErrorPrototype, function, filePath, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewError(string message,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
        {
            return new JSException(message, ErrorPrototype, function, filePath, line);
        }

        public void ReportError(Exception ex)
        {
            var cx = this[KeyStrings.console];
            if (cx.IsUndefined)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return;
            }

            var log = cx[KeyStrings.log];
            if (log.IsUndefined)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return;
            }
            log.InvokeFunction(new Arguments(cx, new JSString(ex.ToString())));
        }


    }
}
