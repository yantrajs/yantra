using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Enumerators;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{

    [JSRuntime(typeof(JSObjectStatic), typeof(JSObjectPrototype))]
    public partial class JSObject : JSValue
    {

        internal ObjectStatus status = ObjectStatus.None;

        internal UInt32Trie<JSProperty> elements;
        private PropertySequence ownProperties;
        internal CompactUInt32Trie<JSProperty> symbols;

        public override bool BooleanValue => true;

        public override bool IsObject => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsSealed() => (this.status & ObjectStatus.Sealed) > 0;

        internal bool IsSealedOrFrozen() => (this.status & (ObjectStatus.SealedOrFrozen)) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsExtensible() => !((this.status & ObjectStatus.NonExtensible) > 0);


        internal bool IsSealedOrFrozenOrNonExtensible() => ((this.status & ObjectStatus.SealedFrozenNonExtensible) > 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsFrozen() => (this.status & ObjectStatus.Frozen) > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref PropertySequence GetOwnProperties(bool create = true)
        {
            if (ownProperties.IsEmpty && create)
                ownProperties = new PropertySequence(4);
            return ref ownProperties;
        }

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
            ownProperties = new PropertySequence(4);
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        public JSObject(IEnumerable<JSProperty> entries) : this(JSContext.Current?.ObjectPrototype)
        {
            ownProperties = new PropertySequence(4);
            foreach (var p in entries)
            {
                ownProperties[p.key.Key] = p;
            }
        }

        internal static JSObject NewWithProperties()
        {
            var o = new JSObject
            {
                ownProperties = new PropertySequence(4)
            };
            return o;
        }

        internal static JSObject NewWithElements()
        {
            var o = new JSObject
            {
                elements = new UInt32Trie<JSProperty>()
            };
            return o;
        }

        internal static JSObject NewWithPropertiesAndElements()
        {
            var o = new JSObject
            {
                ownProperties = new PropertySequence(4),
                elements = new UInt32Trie<JSProperty>()
            };
            return o;
        }

        internal JSObject AddElement(uint index, JSValue value)
        {
            elements[index] = JSProperty.Property(value);
            return this;
        }

        internal JSObject AddProperty(KeyString key, JSValue value)
        {
            ownProperties[key.Key] = JSProperty.Property(key, value);
            return this;
        }

        internal JSObject AddProperty(KeyString key, JSFunction getter, JSFunction setter)
        {
            ownProperties[key.Key] = JSProperty.Property(key, getter?.f, setter?.f);
            return this;
        }


        internal JSObject AddProperty(JSValue key, JSValue value)
        {
            var k = key.ToKey(true);
            if (k.IsUInt)
            {
                elements[k.Key] = JSProperty.Property(value);
            } else
            {
                ownProperties[k.Key] = JSProperty.Property(k, value);
            }
            return this;
        }

        internal override KeyString ToKey(bool create = true)
        {
            if (!create)
            {
                if (KeyStrings.TryGet(this.ToString(), out var k))
                    return k;
                return KeyStrings.undefined;
            }
            return KeyStrings.GetOrCreate(this.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref JSProperty GetInternalProperty(in KeyString key, bool inherited = true)
        {
            ref var r = ref ownProperties.GetValue(key.Key);
            if (!r.IsEmpty)
                return ref r;
            if (inherited && prototypeChain != null)
                return ref prototypeChain.GetInternalProperty(key, inherited);
            return ref JSProperty.Empty;
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

        internal JSProperty GetInternalProperty(JSSymbol key, bool inherited = true)
        {
            if (symbols != null && symbols.TryGetValue(key.Key.Key, out var r))
            {
                return r;
            }
            if (inherited && prototypeChain != null)
                return prototypeChain.GetInternalProperty(key, inherited);
            return new JSProperty();
        }

        internal override JSFunctionDelegate GetMethod(in KeyString key)
        {
            if (!ownProperties.IsEmpty)
            {
                ref var p = ref ownProperties.GetValue(key.Key);
                if (p.IsValue)
                {
                    var g = p.get;
                    if (g != null)
                        return g.f;
                }
                if(p.IsProperty)
                    return p.get.f;
            }
            if (prototypeChain != null && prototypeChain != this)
                return prototypeChain.GetMethod(key);
            throw JSContext.Current.NewError($"Method {key} not found");
        }

        public override JSValue this[KeyString name] { 
            get => this.GetValue(GetInternalProperty(name)); 
            set {
                ref var p = ref GetInternalProperty(name);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(new Arguments(this, value));
                        return;
                    }
                    return;
                }
                if (this.IsFrozen())
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                ref var ownProperties = ref this.GetOwnProperties();
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
                        p.set.f(new Arguments(this, value));
                        return;
                    }
                    return;
                }
                if (this.IsFrozen())
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                elements = elements ?? (elements = new UInt32Trie<JSProperty>());
                elements[name] = JSProperty.Property(value);
            }
        }

        public override JSValue this[JSSymbol name]
        {
            get => this.GetValue(GetInternalProperty(name));
            set
            {
                var p = GetInternalProperty(name);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(new Arguments(this, value));
                        return;
                    }
                    return;
                }
                if (this.IsFrozen())
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                this.symbols = this.symbols ?? new CompactUInt32Trie<JSProperty>();
                symbols[name.Key.Key] = JSProperty.Property(value);
            }
        }

        public JSValue DefineProperty(JSSymbol name, JSProperty p)
        {
            var key = name.Key.Key;
            var symbols = this.symbols ?? (this.symbols = new CompactUInt32Trie<JSProperty>());
            var old = symbols[key];
            if (!old.IsEmpty)
            {
                if (!old.IsConfigurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            p.key = name.Key;
            symbols[key] = p;
            return JSUndefined.Value;
        }

        public JSValue DefineProperty(KeyString name, JSProperty p)
        {
            var key = name.Key;
            ref var ownProperties = ref GetOwnProperties();
            ref var old = ref ownProperties.GetValue(name.Key);
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
            ref var ownProperties = ref GetOwnProperties();
            foreach (var p in list)
            {
                var key = p.key.Key;
                ref var old = ref ownProperties.GetValue(key);
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
            ref var px = ref GetInternalProperty(KeyStrings.toString);
            if (!px.IsEmpty)
            {
                var v = this.GetValue(px);
                if (v.IsFunction)
                    v = v.InvokeFunction(new Arguments(this));
                if (v == this)
                    return "Stack overflow ...";
                return v.ToString();
            }
            return "[object Object]";
        }

        protected internal override JSString ToJSString()
        {
            return new JSString("[object Object]");
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
                var v = fx.InvokeFunction(new Arguments(this));
                return v.DoubleValue;
            }
        }

        public override int Length {
            get
            {
                var ownp = this.ownProperties;
                if (ownp.IsEmpty)
                {
                    return -1;
                }
                ref var l = ref ownp.GetValue(KeyStrings.length.Key);
                if (!l.IsEmpty)
                {
                    var n = this.GetValue(l);
                    var nvalue = ((uint)n.DoubleValue) >> 0;
                    return (int)nvalue;
                }
                return -1;
            }
            set {
                if (this.IsSealedOrFrozenOrNonExtensible())
                    throw JSContext.Current.NewTypeError($"Cannot modify property length of {this}");
                ref var ownp = ref GetOwnProperties();
                ownp[KeyStrings.length.Key] = JSProperty.Property(KeyStrings.length,new JSNumber(value));
            }
        }

        internal override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            return new KeyEnumerator(this, showEnumerableOnly, inherited);
            //var elements = this.elements;
            //if (elements != null)
            //{
            //    foreach (var (Key, Value) in elements.AllValues)
            //    {
            //        if (showEnumerableOnly)
            //        {
            //            if (!Value.IsEnumerable)
            //                continue;
            //        }
            //        yield return new JSNumber(Key);
            //    }
            //}

            //var ownProperties = this.ownProperties;
            //if (ownProperties != null)
            //{
            //    var en = new PropertySequence.Enumerator(ownProperties);
            //    while(en.MoveNext())
            //    {
            //        var p = en.Current;
            //        if (showEnumerableOnly)
            //        {
            //            if (!p.IsEnumerable)
            //                continue;
            //        }
            //        yield return p.ToJSValue();
            //    }
            //}

            //if (inherited)
            //{
            //    var @base = this.prototypeChain;
            //    if (@base != this && @base != null)
            //    {
            //        foreach (var i in @base.GetAllKeys(showEnumerableOnly))
            //            yield return i;
            //    }
            //}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InternalAddProperty(JSObject target, uint key, JSValue pd)
        {
            var p = new JSProperty();
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
            var elements = target.elements ?? (target.elements = new UInt32Trie<JSProperty>());
            elements[key] = p;
            if (target is JSArray array)
            {
                if (array._length <= key)
                    array._length = key + 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InternalAddProperty(JSObject target, in KeyString key, JSValue pd)
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
                p.get = value as JSFunction;
            }
            p.Attributes = pt;
            ref var ownProperties = ref target.GetOwnProperties();
            ownProperties[p.key.Key] = p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InternalAddProperty(JSObject target, JSSymbol key, JSValue pd)
        {
            var p = new JSProperty
            {
                key = key.Key
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
                p.get = value as JSFunction;
            }
            p.Attributes = pt;
            var symbols = target.symbols ?? (target.symbols = new CompactUInt32Trie<JSProperty>());
            symbols[p.key.Key] = p;
        }


        public override JSValue Delete(KeyString key)
        {
            if (this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
            if (ownProperties.RemoveAt(key.Key))
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSValue Delete(uint key)
        {
            if (this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
            if (elements?.RemoveAt(key) ?? false)
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSBoolean Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            if (value is JSString str)
                if (this.ToString() == str.value)
                    return JSBoolean.True;
            if (DoubleValue == value.DoubleValue)
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSBoolean StrictEquals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            return JSBoolean.False;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw new NotImplementedException("object is not a function");
        }

        internal override JSBoolean Less(JSValue value)
        {
            switch(value)
            {
                case JSString strValue:
                    if (this.ToString().CompareTo(strValue.value) < 0)
                        return JSBoolean.True;
                    break;
            }
            return JSBoolean.False;
        }

        internal override JSBoolean LessOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) <= 0):
                        return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        internal override JSBoolean Greater(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) > 0):
                        return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        internal override JSBoolean GreaterOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return JSBoolean.True;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) >= 0):
                        return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        internal override bool TryGetValue(uint i, out JSProperty value)
        {
            if (elements == null)
            {
                value = new JSProperty();
                return false;
            }
            return elements.TryGetValue(i, out value);
        }

        /// <summary>
        /// Moves elements from `start` to `to`.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="to"></param>
        internal override void MoveElements(int start, int to)
        {
            var elements = this.elements ?? (this.elements = new UInt32Trie<JSProperty>());

            var end = this.Length - 1;
            var diff = to - start;
            if (start > to)
            {

                for (uint i = (uint)start, j = (uint)to; i <= end; i++, j++)
                {
                    if (elements.TryRemove(i, out var p))
                    {
                        elements[j] = p;
                    }
                }
                this.Length += diff;
                return;
            }
            else
            {
                for (int i = end, j = (this.Length + diff - 1); i >= start; i--, j--)
                {
                    if (elements.TryRemove((uint)i, out var p))
                    {
                        elements[(uint)j] = p;
                    }
                }
                this.Length += diff;
            }

        }

        internal override bool TryRemove(uint i, out JSProperty p)
        {
            if(elements == null)
                return base.TryRemove(i, out p);
            return elements.TryRemove(i, out p);
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (this.TryGetClrEnumerator(type, out value))
                return true;
            if (type == typeof(object))
            {
                value = this;
                return true;
            }
            if (type != typeof(Type))
            {
                // if type has default constructor...
                if (this.TryUnmarshal(type, out value))
                    return true;
            }
            return base.ConvertTo(type, out value);
        }

        internal override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        private struct ElementEnumerator : IElementEnumerator
        {
            private readonly JSObject @object;
            readonly IEnumerator<(uint Key, JSProperty Value)> en;
            public ElementEnumerator(JSObject @object)
            {
                this.en = @object.elements?.AllValues.GetEnumerator();
                this.@object = @object;
            }


            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if(en?.MoveNext() ?? false) {
                    var (Key, Value) = en.Current;
                    value = @object.GetValue(Value);
                    index = Key;
                    hasValue = true;
                    return true;
                }
                hasValue = false;
                value = JSUndefined.Value;
                index = 0;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if (en?.MoveNext() ?? false)
                {
                    var (Key, Value) = en.Current;
                    value = @object.GetValue(Value);
                    return true;
                }
                value = JSUndefined.Value;
                return false;
            }

        }

    }
}
