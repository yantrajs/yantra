using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{

    public delegate JSValue JSFunctionDelegate(JSValue thisValue, JSValue arguments);

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

            (JSObject p, uint key, Type type) CreatePrototype(JSString name, Type type)
            {
                var r = new JSObject();
                r.prototype = obj ?? r;
                this[name] = r;
                return (r, name.Key, type);
            }

            // create object prototype...
            var objProtoType = CreatePrototype(KeyStrings.Object, typeof(JSObject));
            obj = objProtoType.p;
            var str = CreatePrototype(KeyStrings.String, typeof(JSString));
            var number = CreatePrototype(KeyStrings.Number, typeof(JSNumber));
            var array = CreatePrototype(KeyStrings.Array, typeof(JSArray));
            var function = CreatePrototype(KeyStrings.Function, typeof(JSFunction));

            SetupPrototype(objProtoType);
            SetupPrototype(number);
            SetupPrototype(array);
            SetupPrototype(str);
            SetupPrototype(function);
        }

        private static BinaryUInt32Map<JSValue> cache = new BinaryUInt32Map<JSValue>();
        private void SetupPrototype((JSObject target, uint key, Type type) prot)
        {
            var (target, key, type) = prot;
            var p = cache.GetOrCreate(key, () => {
                var cp = new JSObject();
                foreach(var f in type.GetFields(
                    System.Reflection.BindingFlags.NonPublic 
                    | System.Reflection.BindingFlags.Static
                    | System.Reflection.BindingFlags.FlattenHierarchy)
                .Where(x => x.FieldType == typeof(JSProperty)))
                {
                    var ks = KeyStrings.GetOrCreate(f.Name);
                    var jsp = (JSProperty)f.GetValue(null);
                    jsp.key = ks;
                    f.SetValue(null,jsp);
                    cp.ownProperties[ks.Key] = jsp;
                }

                return cp;
            });
            foreach(var a in p.ownProperties.AllValues())
            {
                target.ownProperties[a.Key] = a.Value;
            }
        }

        public JSValue CreateNumber(double n)
        {
            var v = new JSNumber(n);
            v.prototype = this[KeyStrings.Number];
            return v;
        }

        public JSString CreateString(string value)
        {
            var v = new JSString(value, 0);
            v.prototype = this[KeyStrings.String];
            return v;
        }

        public JSFunction CreateFunction(JSFunctionDelegate fx)
        {
            var v = new JSFunction(fx);
            v.prototype = this[KeyStrings.Function];
            return v;
        }

    }
}
