using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public class PropertySequence
    {

        private BinaryUInt32Map<int> map = new BinaryUInt32Map<int>();
        private List<JSProperty> properties = new List<JSProperty>();

        public IEnumerable<(uint Key,JSProperty Value)> AllValues()
        {
            foreach(var p in properties)
            {
                if (p.Attributes != JSPropertyAttributes.Deleted)
                    yield return (p.key.Key, p);
            }
        }
        public void Update(Func<uint, JSProperty, (bool update, JSProperty v)> func)
        {
            int i = 0;
            foreach(var p in properties.ToList())
            {
                var update = func((p.key.Key), p);
                if (update.update)
                {
                    properties[i] = update.v;
                }
                i++;
            }
        }
        public bool RemoveAt(uint key)
        {
            if(map.TryGetValue(key, out var pkey))
            {
                // move all properties up...
                properties[pkey] = new JSProperty { Attributes = JSPropertyAttributes.Deleted };
            }
            return false;
        }
        public bool TryGetValue(uint key, out JSProperty obj)
        {
            if(map.TryGetValue(key, out var pkey))
            {
                obj = properties[pkey];
                return obj.Attributes != JSPropertyAttributes.Deleted;
            }
            obj = new JSProperty();
            return false;
        }

        public JSProperty this [uint key]
        {
            get
            {
                if(map.TryGetValue(key, out var pkey))
                {
                    return properties[pkey];
                }
                return new JSProperty();
            }
            set { 
                if(map.TryGetValue(key,out var pkey))
                {
                    properties[pkey] = value;
                    return;
                }
                pkey = properties.Count;
                map[key] = pkey;
                properties.Add(value);
            }
        }

    }

    public class JSObject : JSValue
    {
        public static readonly KeyString KeyToJSON = "toJSON";

        protected JSObject(JSValue prototype) : base(prototype)
        {
        }

        public JSObject() : this(JSContext.Current?.ObjectPrototype)
        {
            
        }

        public JSObject(params JSProperty[] entries) : this(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new PropertySequence();
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        public JSObject(IEnumerable<JSProperty> entries) : this(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new PropertySequence();
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        public IEnumerable<(string key, JSValue value)> Entries
        {
            get
            {
                foreach(var item in this.GetInternalEntries())
                {
                    yield return (item.key.ToString(), item.value);
                }
            }
        }

        internal BinaryUInt32Map<JSProperty> elements;
        internal PropertySequence ownProperties;


        public JSValue DefineProperty(KeyString name, JSProperty p)
        {
            var key = name.Key;
            var ownProperties = this.ownProperties ?? (this.ownProperties = new PropertySequence());
            var old = ownProperties[key];
            if (!old.IsEmpty)
            {
                if (!old.IsConfigurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            p.key = name;
            ownProperties[key] = p;
            return JSUndefined.Value;
        }

        public void DefineProperties(params JSProperty[] list)
        {
            var ownProperties = this.ownProperties ?? (this.ownProperties = new PropertySequence());
            foreach (var p in list)
            {
                var key = p.key.Key;
                var old = ownProperties[key];
                if (!old.IsEmpty)
                {
                    if (!old.IsConfigurable)
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                ownProperties[key] = p;
            }
        }

        public static JSValue PropertyIsEnumerable(JSValue t,params JSValue[] a)
        {
            switch (t)
            {
                case JSUndefined _:
                case JSNull _:
                    throw JSContext.Current.Error("Cannot convert undefined or null to object");
            }
            if (a.Length > 0)
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
            var all = this.GetEntries().Select((e) => $"{e.Key}: {e.Value.ToDetailString()}");
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
        public static JSValue ToString(JSValue t,params JSValue[] a) => new JSString("[object Object]");


        [Prototype("__proto__", MemberType.Get)]
        internal static JSValue PrototypeGet(JSValue t,params JSValue[] a)
        {
            return t.prototypeChain;
        }

        [Prototype("__proto__", MemberType.Set)]
        internal static JSValue PrototypeSet(JSValue t,params JSValue[] a)
        {
            return t.prototypeChain = a[0];
        }


        [Static("create")]
        internal static JSValue StaticCreate(JSValue t,params JSValue[] a)
        {
            var p = a[0];
            if (p.IsUndefined)
                p = JSContext.Current.ObjectPrototype;
            return new JSObject(p);
        }

        [Static("assign")]
        internal static JSValue Assign(JSValue t,params JSValue[] a)
        {
            if (a.Length == 0)
                throw JSContext.Current.TypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            var first = a[0];
            if (first is JSNull || first is JSUndefined)
                throw JSContext.Current.TypeError(JSError.Cannot_convert_undefined_or_null_to_object);
            if (a.Length == 1 || !(first is JSObject @firstObject))
                return first;
            var second = a[1];
            if (!(second is JSObject @object))
                return first;
            if (@object.ownProperties != null)
            {
                foreach (var item in @object.ownProperties.AllValues())
                {
                    firstObject.ownProperties[item.Key] = item.Value;
                }
            }
            return first;
        }

        [Static("entries")]
        internal static JSValue StaticEntries(JSValue t,params JSValue[] a)
        {
            var target = a[0];
            switch(target)
            {
                case JSNull @null:
                case JSUndefined undefined:
                    throw JSContext.Current.TypeError(JSError.Cannot_convert_undefined_or_null_to_object);
                case JSObject _:
                    break;
                default:
                    return new JSArray();
            }
            var r = new JSArray();
            foreach(var entry in target.GetInternalEntries())
            {
                r.elements[r._length++] = JSProperty.Property(new JSArray(entry.key.ToJSValue(), entry.value));
            }
            return r;
        }

        [Static("freeze")]
        internal static JSValue Freeze(JSValue t,params JSValue[] a)
        {
            return t;
        }

        internal static JSValue HasOwnProperty(JSValue t,params JSValue[] a)
        {
            return t;
        }

        internal static JSValue IsPrototypeOf(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("defineProperties")]
        internal static JSValue _DefineProperties(JSValue t,params JSValue[] a)
        {
            if (!(a[0] is JSObject target))
                throw JSContext.Current.TypeError("Object.defineProperty called on non-object");
            var pds = a[1];
            if (pds is JSUndefined || pds is JSNull)
                throw JSContext.Current.TypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(pds is JSObject pdObject))
                return target;

            foreach(var pd in pdObject.GetEntries())
            {
                if (pd.Value is JSObject property)
                {
                    InternalAddProperty(target, pd.Key, property);
                }
            }

            return target;
        }

        [Static("defineProperty")]
        internal static JSValue _DefineProperty(JSValue t,params JSValue[] a)
        {
            if (!(a[0] is JSObject target))
                throw new JSException("Object.defineProperty called on non-object");
            var key = a[1].ToString();
            if (!(a[2] is JSObject pd))
                throw new JSException("Property Description must be an object");
            InternalAddProperty(target, key, pd);
            return target;
        }

        private static void InternalAddProperty(JSObject target, string key, JSObject pd)
        {
            var p = new JSProperty
            {
                key = key
            };
            var value = pd[KeyStrings.value];
            var get = pd[KeyStrings.get] as JSFunction;
            var set = pd[KeyStrings.set] as JSFunction;
            var pt = JSPropertyAttributes.Empty;
            if (pd[KeyStrings.configurable].BooleanValue)
                pt |= JSPropertyAttributes.Configurable;
            if (pd[KeyStrings.enumerable].BooleanValue)
                pt |= JSPropertyAttributes.Enumerable;
            if (pd[KeyStrings.@readonly].BooleanValue)
                pt |= JSPropertyAttributes.Readonly;
            if (get != null)
            {
                pt |= JSPropertyAttributes.Property;
                p.get = get;
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
                p.set = set;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                p.value = value;
            }
            p.Attributes = pt;
            target.ownProperties[p.key.Key] = p;
        }

        [Static("fromEntries")]

        internal static JSValue _FromEntries(JSValue t,params JSValue[] a)
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
                    if (!(vi.value is JSArray ia))
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

        internal static JSValue _Is(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("isExtensible")]
        internal static JSValue _IsExtensible(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("isFrozen")]
        internal static JSValue _IsFrozen(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("isSealed")]
        internal static JSValue _IsSealed(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("keys")]
        internal static JSValue _Keys(JSValue t,params JSValue[] a)
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
        internal static JSValue _PreventExtensions(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("seal")]

        internal static JSValue _Seal(JSValue t,params JSValue[] a)
        {
            var first = a[0];
            if (!(first is JSObject @object))
                return first;
            @object.ownProperties.Update((x, v) =>
            {
                v.Attributes &= ~(JSPropertyAttributes.Configurable);
                return (true, v);
            });
            return first;
        }

        [Static("setPrototypeOf")]
        internal static JSValue _SetPrototypeOf(JSValue t,params JSValue[] a)
        {
            var first = a[0];
            if (!(first is JSObject @object))
                return first;
            first.prototypeChain = a[1] as JSObject;
            return first;
        }

        [Static("values")]
        internal static JSValue _Values(JSValue t,params JSValue[] a)
        {
            var first = a[0];
            if (first is JSUndefined)
                throw JSContext.Current.TypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            if (!(first is JSObject jobj))
                return new JSArray();
            return new JSArray(jobj.GetEntries().Select(x => x.Value));
        }

        [Static("getOwnPropertyDescriptor")]
        internal static JSValue _GetOwnPropertyDescriptor(JSValue t,params JSValue[] a)
        {
            var first = a[0];
            if (first is JSUndefined)
                throw JSContext.Current.TypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            return t;
        }

        [Static("getOwnPropertyDescriptors")]
        internal static JSValue _GetOwnPropertyDescriptors(JSValue t,params JSValue[] a)
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
        internal static JSValue _GetOwnPropertyNames(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("getOwnPropertySymbols")]
        internal static JSValue _GetOwnPropertySymbols(JSValue t,params JSValue[] a)
        {
            return t;
        }

        [Static("getPrototypeOf")]
        internal static JSValue _GetPrototypeOf(JSValue t,params JSValue[] a)
        {
            var target = a[0];
            var p = target.prototypeChain;
            return p;
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
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            if (value is JSString str)
                if (this.ToString() == str.value)
                    return JSContext.Current.True;
            if (DoubleValue == value.DoubleValue)
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSContext.Current.True;
            return JSContext.Current.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
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
