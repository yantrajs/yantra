using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

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

        public readonly JSValue StringPrototype;
        public readonly JSValue FunctionPrototype;

        public readonly JSValue NumberPrototype;

        public readonly JSValue ObjectPrototype;

        public readonly JSValue ArrayPrototype;

        public readonly JSValue BooleanPrototype;

        public readonly JSValue TypeErrorPrototype;

        public readonly JSValue ErrorPrototype;

        public readonly JSValue RangeErrorPrototype;

        public readonly JSValue DatePrototype;

        public readonly JSFunction String;

        public readonly JSFunction Function;

        public readonly JSFunction Number;

        public readonly JSFunction Object;

        public readonly JSFunction Array;

        public readonly JSFunction Boolean;

        public readonly JSFunction Error;

        public readonly JSFunction RangeError;

        public readonly JSFunction Date;

        public readonly JSFunction TypeError;
        public readonly JSBoolean True;

        public readonly JSBoolean False;

        public readonly JSNumber NaN;

        public readonly JSNumber One;

        public readonly JSNumber Two;

        public readonly JSNumber Zero;

        public readonly JSObject JSON;

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

            (JSFunction function, JSValue prototype) CreateFrom(KeyString name, Type type, JSValue baseType = null)
            {
                return CreatePrototype(name, () => Bootstrap.Create(name, type), baseType);
            }


            (JSFunction function,JSValue prototype) CreatePrototype(KeyString name, Func<JSFunction> factory, JSValue prototypeChain = null)
            {
                var r = new JSFunction(JSFunction.empty, name.ToString());
                this[name] = r;
                r.prototypeChain = prototypeChain ?? ObjectPrototype;
                var cached = cache.GetOrCreate(name.Key, () =>
                {
                    lock (cache) { return factory(); }
                });
                r.f = cached.f;
                var target = r.prototype.ownProperties;
                foreach(var p in cached.prototype.ownProperties.AllValues())
                {
                    target[p.Key] = p.Value;
                }
                var ro = r.ownProperties;
                foreach(var p in cached.ownProperties.AllValues())
                {
                    /// this is the case when we do not
                    /// want to overwrite Function.prototype
                    if (p.Key != KeyStrings.prototype.Key)
                    {
                        ro[p.Key] = p.Value;
                    }
                }
                return (r,r.prototype);
            }

            // create object prototype...
            (Object, ObjectPrototype) =  CreateFrom(KeyStrings.Object, typeof(JSObject));
            (Array, ArrayPrototype) = CreateFrom(KeyStrings.Array, typeof(JSArray));
            (String, StringPrototype) = CreateFrom(KeyStrings.String, typeof(JSString));
            (Number, NumberPrototype) = CreateFrom(KeyStrings.Number, typeof(JSNumber));
            (Function, FunctionPrototype) = CreateFrom(KeyStrings.Function, typeof(JSFunction));
            (Boolean, BooleanPrototype) = CreatePrototype(KeyStrings.Boolean, JSBoolean.Create);
            (Error, ErrorPrototype) = CreateFrom(JSError.KeyError, typeof(JSError));
            (TypeError, TypeErrorPrototype) = CreateFrom(JSTypeError.KeyTypeError, typeof(JSError), ErrorPrototype);
            (RangeError, RangeErrorPrototype) = CreateFrom(JSTypeError.KeyRangeError, typeof(JSError), ErrorPrototype);
            (Date, DatePrototype) = CreateFrom(KeyStrings.Date, typeof(JSDate));
            True = new JSBoolean(true, BooleanPrototype);
            False = new JSBoolean(false, BooleanPrototype);
            NaN = new JSNumber(double.NaN, NumberPrototype);
            One = new JSNumber(1, NumberPrototype);
            Zero = new JSNumber(0, NumberPrototype);
            Two = new JSNumber(2, NumberPrototype);
            CreatePrototype(JSJSON.JSON, JSJSON.Create);
            JSON = this[JSJSON.JSON] as JSObject;
        }
        private static BinaryUInt32Map<JSFunction> cache = new BinaryUInt32Map<JSFunction>();


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

        public JSFunction CreateFunction(JSFunctionDelegate fx)
        {
            var v = new JSFunction(fx);
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
        private JSException NewError(string message, JSValue prototype)
        {
            return new JSException(message, prototype);
        }

    }
}
