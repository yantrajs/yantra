using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class JSObject : JSValue
    {
        public static readonly KeyString KeyToJSON = "toJSON";

        public JSObject() : base(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
        }

        public JSObject(params JSProperty[] entries) : base(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        public JSObject(IEnumerable<JSProperty> entries) : base(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new BinaryUInt32Map<JSProperty>();
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        protected JSObject(JSValue prototype) : base(prototype)
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
                var key = p.key.Key;
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

        public static JSValue PropertyIsEnumerable(JSValue t, JSArguments a)
        {
            switch (t)
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


        public override string ToDetailString()
        {
            var all = Entries.Select((e) => $"{e.Key}: {e.Value.ToDetailString()}");
            return $"{{ {string.Join(", ", all)} }}";
        }

        public override double DoubleValue{
            get {
                var fx = this[KeyStrings.valueOf];
                if (fx is JSUndefined)
                    return NumberParser.CoerceToNumber(this.ToString());
                var v = fx.InvokeFunction(this, JSArguments.Empty);
                return v.DoubleValue;
            }
        }


        [Prototype("toString")]
        public static JSValue ToString(JSValue t, JSArguments a) => new JSString("[object Object]");


        [Prototype("__proto__", MemberType.Get)]
        internal static JSValue PrototypeGet(JSValue t, JSArguments a)
        {
            return t.prototypeChain;
        }

        [Prototype("__proto__", MemberType.Set)]
        internal static JSValue PrototypeSet(JSValue t, JSArguments a)
        {
            return t.prototypeChain;
        }


        [Static("create")]
        internal static JSValue StaticCreate(JSValue t, JSArguments a)
        {
            var p = a[0];
            if (p.IsUndefined)
                p = JSContext.Current.ObjectPrototype;
            return new JSObject(p);
        }

        [Static("assign")]
        internal static JSValue Assign(JSValue t, JSArguments a)
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

        [Static("entries")]
        internal static JSValue StaticEntries(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("freeze")]
        internal static JSValue Freeze(JSValue t, JSArguments a)
        {
            return t;
        }

        internal static JSValue HasOwnProperty(JSValue t, JSArguments a)
        {
            return t;
        }

        internal static JSValue IsPrototypeOf(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("defineProperties")]
        internal static JSValue _DefineProperties(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("defineProperty")]
        internal static JSValue _DefineProperty(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("fromEntries")]

        internal static JSValue _FromEntries(JSValue t, JSArguments a)
        {
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

        [Static("is")]

        internal static JSValue _Is(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("isExtensible")]
        internal static JSValue _IsExtensible(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("isFrozen")]
        internal static JSValue _IsFrozen(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("isSealed")]
        internal static JSValue _IsSealed(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("keys")]
        internal static JSValue _Keys(JSValue t, JSArguments a)
        {
            var first = a[0];
            if (first is JSUndefined)
                throw JSContext.Current.TypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            return new JSArray(jobj.ownProperties
                .AllValues()
                .Where(x => x.Value.IsEnumerable)
                .Select(x => new JSString(x.Value.ToString())));
        }

        [Static("preventExtensions")]
        internal static JSValue _PreventExtensions(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("seal")]

        internal static JSValue _Seal(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("setPrototypeOf")]
        internal static JSValue _SetPrototypeOf(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("values")]
        internal static JSValue _Values(JSValue t, JSArguments a)
        {
            var first = a[0];
            if (first is JSUndefined)
                throw JSContext.Current.TypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            return new JSArray(jobj.Entries.Select(x => x.Value));
        }

        [Static("getOwnPropertyDescriptor")]
        internal static JSValue _GetOwnPropertyDescriptor(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("getOwnPropertyDescriptors")]
        internal static JSValue _GetOwnPropertyDescriptors(JSValue t, JSArguments a)
        {
            var first = a[0];
            if (first is JSUndefined)
                throw JSContext.Current.TypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            return new JSObject(jobj.ownProperties.AllValues().Select(x => 
                JSProperty.Property(x.Value.key, x.Value.ToJSValue())
            ));
        }

        [Static("getOwnPropertyNames")]
        internal static JSValue _GetOwnPropertyNames(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("getOwnPropertySymbols")]
        internal static JSValue _GetOwnPropertySymbols(JSValue t, JSArguments a)
        {
            return t;
        }

        [Static("getPrototypeOf")]
        internal static JSValue _GetPrototypeOf(JSValue t, JSArguments a)
        {
            return t;
        }

        public override JSValue AddValue(JSValue value)
        {
            return new JSString(this.ToString() + value.ToString());
        }

        public override JSValue AddValue(double value)
        {
            return new JSString(this.ToString() + value.ToString());
        }

        public override JSValue AddValue(string value)
        {
            return new JSString(this.ToString() + value);
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (value is JSString str)
                if (this.ToString() == str.value)
                    return JSContext.Current.True;
            if (DoubleValue == value.DoubleValue)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue, JSArguments args)
        {
            throw new NotImplementedException("object is not a function");
        }

        internal override JSBoolean Less(JSValue value)
        {
            switch(value)
            {
                case JSString strValue:
                    if (this.ToString().CompareTo(strValue.value) < 0)
                        return JSContext.Current.True;
                    break;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) <= 0):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) > 0):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) >= 0):
                        return JSContext.Current.True;
            }
            return JSContext.Current.False;
        }
    }
}
