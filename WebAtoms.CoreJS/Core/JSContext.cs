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

    public delegate JSValue JSFunctionDelegate(JSValue thisValue, JSArray arguments);

    public class JSContext: JSObject, IDisposable
    {

        static AsyncLocal<JSContext> _current = new AsyncLocal<JSContext>();

        public void Dispose()
        {
            _current.Value = null;
        }

        public JSValue StringPrototype { get; }
        public JSValue FunctionPrototype { get; }
        
        public JSValue NumberPrototype { get; }

        public JSValue ObjectPrototype { get; }
        
        public JSValue ArrayPrototype { get; }

        public JSValue BooleanPrototype { get; }

        public JSValue TypeErrorPrototype { get; }

        public JSValue ErrorPrototype { get; }

        public JSValue RangeErrorPrototype { get; }

        public JSBoolean True { get; }

        public JSBoolean False { get; }

        public JSNumber NaN => new JSNumber(double.NaN);

        public JSNumber One { get; }

        public JSNumber Two { get; }

        public JSNumber Zero { get; }

        public JSObject JSON { get; }

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
            JSObject obj = null;

            _current.Value = this;

            JSValue CreatePrototype(JSName name, Func<JSFunction> factory, JSValue prototypeChain = null)
            {
                var r = new JSFunction(JSFunction.empty, name.ToString());
                this[name] = r;
                r.prototypeChain = prototypeChain ?? obj;
                var cached = cache.GetOrCreate(name.Key.Key, factory);
                var target = r.prototype.ownProperties;
                foreach(var p in cached.prototype.ownProperties.AllValues())
                {
                    target[p.Key] = p.Value;
                }
                var ro = r.ownProperties;
                foreach(var p in cached.ownProperties.AllValues())
                {
                    ro[p.Key] = p.Value;
                }
                return r.prototype;
            }

            // create object prototype...
            ObjectPrototype =  CreatePrototype(KeyStrings.Object, JSObject.Create);
            StringPrototype = CreatePrototype(KeyStrings.String, JSString.Create);
            NumberPrototype = CreatePrototype(KeyStrings.Number, JSNumber.Create);
            ArrayPrototype = CreatePrototype(KeyStrings.Array, JSArray.Create);
            FunctionPrototype = CreatePrototype(KeyStrings.Function, JSFunction.Create);
            BooleanPrototype = CreatePrototype(KeyStrings.Boolean, JSBoolean.Create);
            ErrorPrototype = CreatePrototype(JSError.KeyError, JSError.Create);
            TypeErrorPrototype = CreatePrototype(JSTypeError.KeyTypeError, JSTypeError.Create, ErrorPrototype);
            RangeErrorPrototype = CreatePrototype(JSTypeError.KeyRangeError, JSTypeError.Create, ErrorPrototype);

            True = new JSBoolean(true, BooleanPrototype);
            False = new JSBoolean(false, BooleanPrototype);
            // NaN = new JSNumber(double.NaN, NumberPrototype);
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
        internal JSException TypeError(string message)
        {
            return Error(message, TypeErrorPrototype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException RangeError(string message)
        {
            return Error(message, RangeErrorPrototype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSException Error(string message)
        {
            return Error(message, ErrorPrototype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JSException Error(string message, JSValue prototype)
        {
            return new JSException(message, prototype);
        }

    }
}
