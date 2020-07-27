using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSObject : JSValue
    {
        public static readonly KeyString KeyToJSON = "toJSON";
        public JSObject(): base(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
        }

        public JSObject(params JSProperty[] entries) : base(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
            foreach(var p  in entries)
            {
                ownProperties[p.key.Key.Key] = p;
            }
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

        public void DefineProperties(params JSProperty[] list)
        {
            foreach (var p in list)
            {
                var key = p.key.Key.Key;
                var old = this.ownProperties[key];
                if (!old.IsEmpty)
                {
                    if (!old.IsConfigurable)
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                this.ownProperties[key] = p;
            }
        }

        public static JSValue PropertyIsEnumerable(JSValue t, JSArray a)
        {
            switch(t)
            {
                case JSUndefined _:
                case JSNull _:
                    throw JSContext.Current.Error("Cannot convert undefined or null to object");
            }
            if (a._length > 0)
            {
                var text = a[0].ToString();
                var px = t.GetInternalProperty(text, false);
                if (!px.IsEmpty && px.IsEnumerable)
                    return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        public static JSValue Create(JSValue t, JSArray a)
        {
            var p = a[0];
            if (p.IsUndefined)
                p = JSContext.Current.ObjectPrototype;
            return new JSObject(p);
        }

        public override string ToString()
        {
            return "[object object]";
        }

        public override string ToDetailString()
        {
            var all = Entries.Select((e) => $"{e.Key}: {e.Value.ToDetailString()}");
            return $"{{ {string.Join(", ",all)} }}";
        }

        public static JSValue ToString(JSValue t, JSArray a) => new JSString(t.ToString());

        internal static JSValue StaticCreate(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSValue _Assign(JSValue t, JSArray a)
        {
            if (a._length == 0)
                throw JSContext.Current.Error(JSError.Cannot_convert_undefined_or_null_to_object);
            var first = a[0];
            if (first is JSNull || first is JSUndefined)
                throw JSContext.Current.Error(JSError.Cannot_convert_undefined_or_null_to_object);
            if (a._length == 1 || !(first is JSObject))
                return first;
            var second = a[1];
            if (!(second is JSObject))
                return first;
            foreach(var item in second.ownProperties.AllValues())
            {
                first.ownProperties[item.Key] = item.Value;
            }
            return first;
        }

        internal static JSValue _Entries(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSValue Freeze(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSValue HasOwnProperty(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSValue IsPrototypeOf(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSValue _DefineProperties(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSValue _DefineProperty(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty, "Object");
            var p = r.prototype;
            
            p.DefineProperties(
               
                JSProperty.Property(KeyStrings.__proto__,
                    get: (t, a) => t.prototypeChain,
                    set: (t, a) => t.prototypeChain = a[0], JSPropertyAttributes.Property
                ),


                JSProperty.Function("hasOwnProperty", HasOwnProperty),
                JSProperty.Function("isPrototypeOf", IsPrototypeOf),
                JSProperty.Function("propertyIsEnumerable", PropertyIsEnumerable),

                JSProperty.Function(KeyStrings.toString, ToString)
            );

            r.DefineProperties(
                JSProperty.Function("assign", _Assign),
                JSProperty.Function("create", StaticCreate),
                JSProperty.Function("defineProperties", _DefineProperties),
                JSProperty.Function("defineProperty", _DefineProperty),
                JSProperty.Function("entries", _Entries),
                JSProperty.Function("freeze", Freeze),
                JSProperty.Function("fromEntries", _FromEntries),
                JSProperty.Function("getOwnPropertyDescriptor", _GetOwnPropertyDescriptor),
                JSProperty.Function("getOwnPropertyDescriptors", _GetOwnPropertyDescriptors),
                JSProperty.Function("getOwnPropertyNames", _GetOwnPropertyNames),
                JSProperty.Function("getOwnPropertySymbols", _GetOwnPropertySymbols),
                JSProperty.Function("getPrototypeOf", _GetPrototypeOf),
                JSProperty.Function("is", _Is),
                JSProperty.Function("isExtensible", _IsExtensible),
                JSProperty.Function("isFrozen", _IsFrozen),
                JSProperty.Function("isSealed", _IsSealed),
                JSProperty.Function("keys", _Keys),
                JSProperty.Function("preventExtensions", _PreventExtensions),
                JSProperty.Function("seal", _Seal),
                JSProperty.Function("setPrototypeOf", _SetPrototypeOf),
                JSProperty.Function("values", _Values)
            );


            return r;
        }
        internal static JSValue _FromEntries(JSValue t, JSArray a)
        {
            if (a._length == 0)
            {
                throw JSContext.Current.TypeError(JSTypeError.NotIterable("undefined"));
            }
            var v = a[0];
            if (v is JSUndefined || v is JSNull)
            {
                throw JSContext.Current.TypeError(JSTypeError.NotIterable("undefined"));
            }
            var r = new JSObject();
            if ((v is JSArray va))
            {
                foreach(var item in va.elements.AllValues())
                {
                    var vi = item.Value;
                    if (!(vi is JSArray ia))
                        throw JSContext.Current.TypeError(JSTypeError.NotEntry(vi));
                    var first = ia[0].ToString();
                    var second = ia[1];
                    r.DefineProperty(first, JSProperty.Property(first, second,
                        JSPropertyAttributes.EnumerableConfigurableValue));
                }
            }
            return r;
        }

        internal static JSValue _Is(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _IsExtensible(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _IsFrozen(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _IsSealed(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _Keys(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _PreventExtensions(JSValue t, JSArray a)
        {
            return t;
        }


        internal static JSValue _Seal(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _SetPrototypeOf(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _Values(JSValue t, JSArray a)
        {
            return t;
        }

        internal static JSValue _GetOwnPropertyDescriptor(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _GetOwnPropertyDescriptors(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _GetOwnPropertyNames(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _GetOwnPropertySymbols(JSValue t, JSArray a)
        {
            return t;
        }
        internal static JSValue _GetPrototypeOf(JSValue t, JSArray a)
        {
            return t;
        }

    }
}
