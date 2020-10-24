using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core.Enumerators;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{

    [JSRuntime(typeof(JSObjectStatic), typeof(JSObjectPrototype))]
    public partial class JSObject : JSValue
    {

        internal ObjectStatus status = ObjectStatus.None;

        internal UInt32Trie<JSProperty> elements;
        internal PropertySequence ownProperties;
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

        internal static JSObject NewWithProperties()
        {
            var o = new JSObject
            {
                ownProperties = new PropertySequence()
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
                ownProperties = new PropertySequence(),
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
                ownProperties[k.Key] = JSProperty.Property(value);
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
        internal JSProperty GetInternalProperty(in KeyString key, bool inherited = true)
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

        public override JSValue this[KeyString name] { 
            get => this.GetValue(GetInternalProperty(name)); 
            set {
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
                    v = v.InvokeFunction(new Arguments(this));
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
                var v = fx.InvokeFunction(new Arguments(this));
                return v.DoubleValue;
            }
        }

        public override int Length {
            get
            {
                var ownp = this.ownProperties;
                if (ownp == null)
                {
                    return -1;
                }
                var l = ownp[KeyStrings.length.Key];
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
                var ownp = this.ownProperties ?? (this.ownProperties = new PropertySequence());
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
            }
            p.Attributes = pt;
            var ownProperties = target.ownProperties ?? (target.ownProperties = new PropertySequence());
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
            }
            p.Attributes = pt;
            var symbols = target.symbols ?? (target.symbols = new CompactUInt32Trie<JSProperty>());
            symbols[p.key.Key] = p;
        }


        public override JSValue Delete(KeyString key)
        {
            if (this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
            if (ownProperties?.RemoveAt(key.Key) ?? false)
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
            if (type.IsAssignableFrom(typeof(JSObject)))
            {
                value = this;
                return true;
            }
            if (type == typeof(object))
            {
                value = this;
                return true;
            }
            // if type has default constructor...
            if (this.TryUnmarshal(type, out value))
                return true;
            return base.ConvertTo(type, out value);
        }

        internal override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        private struct ElementEnumerator : IElementEnumerator
        {
            private readonly JSObject @object;
            IEnumerator<(uint Key, JSProperty Value)> en;
            public ElementEnumerator(JSObject @object)
            {
                this.en = @object.elements?.AllValues.GetEnumerator();
                this.@object = @object;
            }


            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if(en?.MoveNext() ?? false) {
                    var c = en.Current;
                    value = @object.GetValue(c.Value);
                    index = c.Key;
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
                    var c = en.Current;
                    value = @object.GetValue(c.Value);
                    return true;
                }
                value = JSUndefined.Value;
                return false;
            }

        }

    }
}
