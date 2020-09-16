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

        public readonly JSObject JSON;

        public readonly JSMath Math;

        public readonly JSFunction Map;

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

            (JSFunction function, JSObject prototype) CreateFrom(KeyString name, Type type, JSObject baseType = null)
            {
                var r = new JSFunction(JSFunction.empty, name.ToString());
                lock (cache)
                {
                    ownProperties[name.Key] = JSProperty.Property(r, JSPropertyAttributes.ConfigurableReadonlyValue);
                    r.prototypeChain = baseType ?? ObjectPrototype;
                    var cached = cache.GetOrCreate(name.Key, () => Bootstrap.Create(name, type));
                    r.f = cached.f;
                    var target = r.prototype.ownProperties;
                    foreach (var p in cached.prototype.ownProperties.AllValues())
                    {
                        target[p.Key] = p.Value;
                    }
                    var ro = r.ownProperties;
                    foreach (var p in cached.ownProperties.AllValues())
                    {
                        /// this is the case when we do not
                        /// want to overwrite Function.prototype
                        if (p.Key != KeyStrings.prototype.Key)
                        {
                            ro[p.Key] = p.Value;
                        }
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
            (Boolean, BooleanPrototype) = CreateFrom(KeyStrings.Boolean, typeof(JSBoolean));
            (Error, ErrorPrototype) = CreateFrom(JSError.KeyError, typeof(JSError));
            (TypeError, TypeErrorPrototype) = CreateFrom(JSTypeError.KeyTypeError, typeof(JSError), ErrorPrototype);
            (RangeError, RangeErrorPrototype) = CreateFrom(JSTypeError.KeyRangeError, typeof(JSError), ErrorPrototype);
            (Date, DatePrototype) = CreateFrom(KeyStrings.Date, typeof(JSDate));
            (Map, MapPrototype) = CreateFrom(KeyStrings.Map, typeof(JSMap));
            JSON = CreateInternalObject<JSJSON>(KeyStrings.JSON);
            Math = CreateInternalObject<JSMath>(KeyStrings.Math);
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
        private JSException NewError(string message, JSObject prototype)
        {
            return new JSException(message, prototype);
        }

    }
}
