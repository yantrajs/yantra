using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSObject : JSValue
    {

        public JSObject()
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
        }

        internal static JSProperty hasOwnProperty = new JSProperty
        {
            value = new JSFunction((t, a) =>
            {
                return JSUndefined.Value;
            })
        };

        public JSValue DefineProperty(JSString name, JSProperty p)
        {
            var key = name.Key == 0 ? KeyStrings.GetOrCreate(name.ToString()).Key : name.Key;
            var old = this.ownProperties[key];
            if (!old.IsEmpty)
            {
                if (!old.configurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            p.key = name;
            this.ownProperties[key] = p;
            return JSUndefined.Value;
        }


        public static JSValue ToString(JSValue t, JSArray a) => new JSString(t.ToString());

        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty, "Object");
            var p = r.prototype;
            r.prototype.DefineProperty(KeyStrings.toString, JSProperty.Function(ToString));
            return r;
        }

    }
}
