using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSObjectStatic), typeof(JSObjectPrototype))]
    public partial class JSObject : JSValue
    {
        public static readonly KeyString KeyToJSON = "toJSON";

        public override bool BooleanValue => true;

        public override bool IsObject => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Object;
        }

        internal JSObject(JSObject prototype) : base(prototype)
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

        internal override KeyString ToKey()
        {
            return KeyStrings.GetOrCreate(this.ToString());
        }

        internal BinaryUInt32Map<JSProperty> elements;
        internal PropertySequence ownProperties;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSProperty GetInternalProperty(KeyString key, bool inherited = true)
        {
            if (ownProperties != null && ownProperties.TryGetValue(key.Key, out var r))
            {
                return r;
            }
            if (inherited && prototypeChain != null)
                return prototypeChain.GetInternalProperty(key, inherited);
            return new JSProperty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSProperty GetInternalProperty(uint key, bool inherited = true)
        {
            if (elements != null && elements.TryGetValue(key, out var r))
            {
                return r;
            }
            if (inherited && prototypeChain != null)
                return prototypeChain.GetInternalProperty(key, inherited);
            return new JSProperty();
        }

        public override JSValue this[KeyString name] { 
            get => this.GetValue(GetInternalProperty(name)); 
            set {
                var p = GetInternalProperty(name);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(this, value);
                        return;
                    }
                    return;
                }
                ownProperties = ownProperties ?? (ownProperties = new PropertySequence());
                ownProperties[name.Key] = JSProperty.Property(name, value);
            }
        }

        public override JSValue this[uint name]
        {
            get => this.GetValue(GetInternalProperty(name));
            set
            {
                var p = GetInternalProperty(name);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(this, value);
                        return;
                    }
                    return;
                }
                elements = elements ?? (elements = new BinaryUInt32Map<JSProperty>());
                elements[name] = JSProperty.Property(value);
            }
        }

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

        public override string ToString()
        {
            var px = GetInternalProperty(KeyStrings.toString);
            if (!px.IsEmpty)
            {
                var v = this.GetValue(px);
                if (v.IsFunction)
                    v = v.InvokeFunction(this);
                if (v == this)
                    throw new StackOverflowException();
                return v.ToString();
            }
            return "[object Object]";
        }

        public override string ToDetailString()
        {
            var all = this.GetAllEntries(false).Select((e) => $"{e.Key}: {e.Value.ToDetailString()}");
            return $"{{ {string.Join(", ", all)} }}";
        }

        public override double DoubleValue{
            get {
                var fx = this[KeyStrings.valueOf];
                if (fx.IsUndefined)
                    return NumberParser.CoerceToNumber(this.ToString());
                var v = fx.InvokeFunction(this, JSArguments.Empty);
                return v.DoubleValue;
            }
        }

        internal override IEnumerable<JSValue> AllElements {
            get {
                // if this is an array, it will be handled by an Array...

                // look for length property..

                if (elements == null)
                    yield break;
                var l = this[KeyStrings.length];
                if (l.IsNull || l.IsUndefined)
                    yield break;
                var n = (uint)l.IntValue;
                for (uint i = 0; i < n; i++)
                {
                    yield return this.GetValue(elements[i]);
                }
            }
        }
        internal override IEnumerable<JSValue> GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            var elements = this.elements;
            if (elements != null)
            {
                foreach (var p in elements.AllValues)
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return new JSNumber(p.Key);
                }
            }

            var ownProperties = this.ownProperties;
            if (ownProperties != null)
            {
                foreach (var p in ownProperties.AllValues())
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return p.Value.ToJSValue();
                }
            }

            if (inherited)
            {
                var @base = this.prototypeChain;
                if (@base != this && @base != null)
                {
                    foreach (var i in @base.GetAllKeys(showEnumerableOnly))
                        yield return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InternalAddProperty(JSObject target, uint key, JSValue pd)
        {
            var p = new JSProperty();
            var value = pd[KeyStrings.value];
            var get = pd[KeyStrings.get] as JSFunctionStatic;
            var set = pd[KeyStrings.set] as JSFunctionStatic;
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
            var elements = target.elements ?? (target.elements = new BinaryUInt32Map<JSProperty>());
            elements[key] = p;
            if (target is JSArray array)
            {
                if (array._length <= key)
                    array._length = key + 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InternalAddProperty(JSObject target, KeyString key, JSValue pd)
        {
            var p = new JSProperty
            {
                key = key
            };
            var value = pd[KeyStrings.value];
            var get = pd[KeyStrings.get] as JSFunctionStatic;
            var set = pd[KeyStrings.set] as JSFunctionStatic;
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
            var ownProperties = target.ownProperties ?? (target.ownProperties = new PropertySequence());
            ownProperties[p.key.Key] = p;
        }

        public override JSValue Delete(KeyString key)
        {
            if (ownProperties?.RemoveAt(key.Key) ?? false)
                return JSBooleanPrototype.True;
            return JSBooleanPrototype.False;
        }

        public override JSValue Delete(uint key)
        {
            if (elements?.RemoveAt(key) ?? false)
                return JSBooleanPrototype.True;
            return JSBooleanPrototype.False;
        }

        public override JSBooleanPrototype Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBooleanPrototype.True;
            if (value is JSString str)
                if (this.ToString() == str.value)
                    return JSBooleanPrototype.True;
            if (DoubleValue == value.DoubleValue)
                return JSBooleanPrototype.True;
            return JSBooleanPrototype.False;
        }

        public override JSBooleanPrototype StrictEquals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBooleanPrototype.True;
            return JSBooleanPrototype.False;
        }

        public override JSValue InvokeFunction(JSValue thisValue,params JSValue[] args)
        {
            throw new NotImplementedException("object is not a function");
        }

        internal override JSBooleanPrototype Less(JSValue value)
        {
            switch(value)
            {
                case JSString strValue:
                    if (this.ToString().CompareTo(strValue.value) < 0)
                        return JSBooleanPrototype.True;
                    break;
            }
            return JSBooleanPrototype.False;
        }

        internal override JSBooleanPrototype LessOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBooleanPrototype.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) <= 0):
                        return JSBooleanPrototype.True;
            }
            return JSBooleanPrototype.False;
        }

        internal override JSBooleanPrototype Greater(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) > 0):
                        return JSBooleanPrototype.True;
            }
            return JSBooleanPrototype.False;
        }

        internal override JSBooleanPrototype GreaterOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBooleanPrototype.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) >= 0):
                        return JSBooleanPrototype.True;
            }
            return JSBooleanPrototype.False;
        }
    }
}
