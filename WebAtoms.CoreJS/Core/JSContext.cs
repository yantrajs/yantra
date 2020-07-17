using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{

    public delegate JSValue JSFunctionDelegate(JSValue thisValue, JSValue arguments);

    public class JSContext: JSObject
    {

        public static JSContext Current
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public JSContext()
        {
            JSObject obj = null;

            JSObject CreatePrototype(JSString name)
            {
                var r = new JSObject();
                r.prototype = obj ?? r;
                this[name] = r;
                return r;
            }

            // create object prototype...
            obj = CreatePrototype(KeyStrings.Object);
            var str = CreatePrototype(KeyStrings.String);
            var number = CreatePrototype(KeyStrings.Number);
            var array = CreatePrototype(KeyStrings.Array);
            var function = CreatePrototype(KeyStrings.Function);

            JSObject.SetupPrototype(obj);
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
