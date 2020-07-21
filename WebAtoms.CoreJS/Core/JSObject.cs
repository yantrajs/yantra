using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSObject : JSValue
    {

        public JSObject(): base(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
        }

        protected JSObject(JSValue prototype): base(prototype)
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
        }

        public JSValue DefineProperty(KeyString name, JSProperty p)
        {
            var key = name.Key;
            var old = this.ownProperties[key];
            if (!old.IsEmpty)
            {
                if (!old.IsConfigurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            p.key = name;
            this.ownProperties[key] = p;
            return JSUndefined.Value;
        }

        public static JSValue Create(JSValue t, JSArray a)
        {
            var p = a[0];
            if (p.IsUndefined)
                p = JSContext.Current.ObjectPrototype;
            return new JSObject(p);
        }


        public static JSValue ToString(JSValue t, JSArray a) => new JSString(t.ToString());

        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty, "Object");
            var p = r.prototype;
            p.DefineProperty(KeyStrings.toString, JSProperty.Function(ToString));
            p.DefineProperty(KeyStrings.__proto__, JSProperty.Property(
                get: (t,a) => t.prototypeChain,
                set: (t,a) => t.prototypeChain = a[0],
                JSPropertyAttributes.Property
            ));

            r.DefineProperty("create", JSProperty.Function(Create));


            return r;
        }

    }
}
