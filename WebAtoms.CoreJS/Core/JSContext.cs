using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            JSValue CreatePrototype(JSString name, Func<JSFunction> factory)
            {
                var r = new JSFunction(JSFunction.empty, name.ToString());
                this[name] = r;
                r.prototypeChain = obj;
                var cached = cache.GetOrCreate(name.Key, factory);
                var target = r.prototype.ownProperties;
                foreach(var p in cached.prototype.ownProperties.AllValues())
                {
                    target[p.Key] = p.Value;
                }
                return r;
            }

            // create object prototype...
            CreatePrototype(KeyStrings.Object, JSObject.Create);
            CreatePrototype(KeyStrings.String, JSString.Create);
            CreatePrototype(KeyStrings.Number, JSNumber.Create);
            CreatePrototype(KeyStrings.Array, JSArray.Create);
            CreatePrototype(KeyStrings.Function, JSFunction.Create);

        }
        private static BinaryUInt32Map<JSFunction> cache = new BinaryUInt32Map<JSFunction>();


        public JSObject CreateObject()
        {
            var v = new JSObject();
            v.prototypeChain = this[KeyStrings.Object];
            return v;
        }

        public JSValue CreateNumber(double n)
        {
            var v = new JSNumber(n);
            v.prototypeChain = this[KeyStrings.Number];
            return v;
        }

        public JSString CreateString(string value)
        {
            var v = new JSString(value, 0);
            v.prototypeChain = this[KeyStrings.String];
            return v;
        }

        public JSFunction CreateFunction(JSFunctionDelegate fx)
        {
            var v = new JSFunction(fx);
            v.prototypeChain = this[KeyStrings.Function];
            return v;
        }

        public JSArray CreateArray()
        {
            var v = new JSArray();
            v.prototypeChain = this[KeyStrings.Array];
            return v;
        }

    }
}
