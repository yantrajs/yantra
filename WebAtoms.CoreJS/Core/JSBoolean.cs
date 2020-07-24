using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSBoolean : JSValue
    {
        internal readonly bool _value;

        internal JSBoolean(bool _value, JSValue prototype) : base(prototype)
        {
            this._value = _value;
        }

        public static bool IsTrue(JSValue value)
        {
            switch(value)
            {
                case JSString str:
                    return str.Length > 0;
                case JSBoolean bv:
                    return bv._value;
                case JSNumber n:
                    return n.value != 0 && n.value != double.NaN;
            }
            return false;
        }

        public override string ToString()
        {
            return _value ? "true" : "false";
        }

        internal static JSFunction Create()
        {
            var r = new JSFunction((t, a) => IsTrue(a[0]) ? JSContext.Current.True : JSContext.Current.False);
            return r;
        }


    }
}
