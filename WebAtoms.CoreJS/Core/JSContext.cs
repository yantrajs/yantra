using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using WebAtoms.CoreJS.Core.Objects;

namespace WebAtoms.CoreJS.Core
{

    public delegate JSValue JSFunctionDelegate(JSValue thisValue, params JSValue[] arguments);

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

        public readonly JSObject DatePrototype;

        public readonly JSObject MapPrototype;

        public readonly JSObject PromisePrototype;

        public readonly JSFunctionStatic String;

        public readonly JSFunctionStatic Function;

        public readonly JSFunctionStatic Number;

        public readonly JSFunctionStatic Object;

        public readonly JSFunctionStatic Array;

        public readonly JSFunctionStatic Boolean;

        public readonly JSFunctionStatic Error;

        public readonly JSFunctionStatic RangeError;

        public readonly JSFunctionStatic Date;

        public readonly JSFunctionStatic TypeError;

        public readonly JSFunctionStatic Promise;

        public readonly JSObject JSON;

        public readonly JSFunctionStatic Symbol;

        public readonly JSMath Math;

        public readonly JSFunctionStatic Map;

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

        public JSContext()
        {
            
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
                    lock(cache)
                    {
                        return Bootstrap.Create(name, typeof(T));
                    }
                });

                ownProperties[name.Key] = JSProperty.Property(r, JSPropertyAttributes.ConfigurableReadonlyValue);

                foreach(var p in cached.ownProperties.AllValues())
                {
                    r.ownProperties[p.Key] = p.Value;
                }

                return r;
            }

            (Symbol, _) = this.Create<JSSymbol>(KeyStrings.Symbol);
            (Function, FunctionPrototype) = this.Create<JSFunctionStatic>(KeyStrings.Function);
            // create object prototype...
            (Object, ObjectPrototype) =  this.Create<JSObject>(KeyStrings.Object);
            (Array, ArrayPrototype) = this.Create<JSArray>(KeyStrings.Array);
            (String, StringPrototype) = this.Create<JSString>(KeyStrings.String);
            (Number, NumberPrototype) = this.Create<JSNumber>(KeyStrings.Number);
            (Boolean, BooleanPrototype) = this.Create<JSBooleanPrototype>(KeyStrings.Boolean);
            (Error, ErrorPrototype) = this.Create<JSError>(JSError.KeyError);
            (TypeError, TypeErrorPrototype) = this.Create<JSTypeError>(JSTypeError.KeyTypeError, ErrorPrototype);
            (RangeError, RangeErrorPrototype) = this.Create<JSTypeError>(JSTypeError.KeyRangeError, ErrorPrototype);
            (Date, DatePrototype) = this.Create<JSDate>(KeyStrings.Date);
            (Map, MapPrototype) = this.Create<JSMap>(KeyStrings.Map);
            (Promise, PromisePrototype) = this.Create<JSPromise>(KeyStrings.Promise);
            JSON = CreateInternalObject<JSJSON>(KeyStrings.JSON);
            Math = CreateInternalObject<JSMath>(KeyStrings.Math);
        }

        private static BinaryUInt32Map<JSFunctionStatic> cache = new BinaryUInt32Map<JSFunctionStatic>();


        public JSObject CreateObject()
        {
            var v = new JSObject();
            return v;
        }

        public JSValue CreateNumber(double n)
        {
            var v = new JSNumber(n);
            return v;
        }

        public JSString CreateString(string value)
        {
            var v = new JSString(value);
            return v;
        }

        public JSFunctionStatic CreateFunction(JSFunctionDelegate fx)
        {
            var v = new JSFunctionStatic(fx);
            return v;
        }

        public JSArray CreateArray()
        {
            var v = new JSArray();
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewTypeError(string message)
        {
            return NewError(message, TypeErrorPrototype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewRangeError(string message)
        {
            return NewError(message, RangeErrorPrototype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException NewError(string message)
        {
            return NewError(message, ErrorPrototype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JSException NewError(string message, JSObject prototype)
        {
            return new JSException(message, prototype);
        }

    }
}
