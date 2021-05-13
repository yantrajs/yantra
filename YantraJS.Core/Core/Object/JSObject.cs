using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Core;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.Enumerators;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{

    internal delegate void PropertyChangedEventHandler(JSObject sender, (uint keyString, uint index, JSSymbol symbol) index);

    [JSRuntime(typeof(JSObjectStatic), typeof(JSObjectPrototype))]
    public partial class JSObject : JSValue
    {
        private JSPrototype currentPrototype;

        internal override JSObject BasePrototypeObject {
            set {
                prototypeChain = value?.PrototypeObject;
                PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, null));
                currentPrototype?.Dirty();
            }
        }

        internal JSPrototype PrototypeObject
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (currentPrototype ?? (currentPrototype = new JSPrototype(this)));
            }
        }
        
        internal void Dirty()
        {
            PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, null));
        }

        internal ObjectStatus status = ObjectStatus.None;

        // internal long version = 0;
        internal event PropertyChangedEventHandler PropertyChanged;
        private ElementArray elements;
        private PropertySequence ownProperties;
        private UInt32Map<JSProperty> symbols;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ElementArray GetElements(bool create = true)
        {
            //if (elements.IsNull && create)
            //    elements = new UInt32Map<JSProperty>();
            return ref elements;
        }

        public ref UInt32Map<JSProperty> GetSymbols()
        {
            return ref symbols;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref ElementArray CreateElements(uint size = 4)
        {
            //if (elements.IsNull)
            //    elements = new UInt32Map<JSProperty>();
            return ref elements;
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

        public IEnumerable<(string Key, JSValue value)> Entries
        {
            get
            {
                var es = GetElementEnumerator();
                while (es.MoveNext(out var hasValue, out var value, out var index))
                {
                    if(hasValue)
                        yield return (index.ToString(), value);
                }

                var ownProperties = GetOwnProperties();
                for(int i = 0; i< ownProperties.properties.Length; i++)
                {
                    var p = ownProperties.properties[i];
                    JSValue v = null;
                    try {
                        v = this.GetValue(p);
                    } catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    yield return (p.key.ToString(), v);
                }
            }
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject NewWithProperties()
        {
            var o = new JSObject
            {
                ownProperties = new PropertySequence(4)
            };
            return o;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject NewWithElements()
        {
            var o = new JSObject()
            {
                // elements = new ElementArray(4)
            };
            return o;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject NewWithPropertiesAndElements()
        {
            var o = new JSObject
            {
                ownProperties = new PropertySequence(4),
                // elements = new ElementArray(4)
            };
            return o;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSObject AddElement(uint index, JSValue value)
        {
            elements[index] = JSProperty.Property(value);
            return this;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSObject AddProperty(in KeyString key, JSValue value)
        {
            ownProperties[key.Key] = JSProperty.Property(key, value);
            return this;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSObject AddProperty(in KeyString key, JSFunction getter, JSFunction setter)
        {
            ownProperties[key.Key] = JSProperty.Property(key, getter?.f, setter?.f);
            return this;
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSObject AddProperty(JSValue key, JSValue value)
        {
            var k = key.ToKey(true);
            if (k.IsUInt)
            {
                elements[k.Key] = JSProperty.Property(value);
            } else
            {
                ref var op = ref GetOwnProperties(true);
                op[k.Key] = JSProperty.Property(k, value);
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
            var r = ownProperties.GetValue(key.Key);
            if (!r.IsEmpty)
                return r;
            if (inherited && prototypeChain != null)
            {
                r = prototypeChain.GetInternalProperty(key);
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSProperty GetInternalProperty(uint key, bool inherited = true)
        {
            if (elements.TryGetValue(key, out var r))
            {
                return r;
            }
            if (inherited && prototypeChain != null)
                return prototypeChain.GetInternalProperty(key);
            return new JSProperty();
        }

        internal JSProperty GetInternalProperty(JSSymbol key, bool inherited = true)
        {
            if (symbols.TryGetValue(key.Key.Key, out var r))
            {
                return r;
            }
            if (inherited && prototypeChain != null)
                return prototypeChain.GetInternalProperty(key);
            return new JSProperty();
        }

        public override JSValue ValueOf()
        {
            var p = GetInternalProperty(KeyStrings.valueOf, false);
            if (!p.IsEmpty)
            {
                return this.GetValue(p).InvokeFunction(new Arguments(this));
                
            }
            return this;
        }

        public bool HasValueOf(out JSValue value)
        {
            var p = GetInternalProperty(KeyStrings.valueOf, false);
            if (!p.IsEmpty)
            {
                value = this.GetValue(p).InvokeFunction(new Arguments(this));
                return true;
            }
            value = null;
            return false;

        }

        //public override JSValue AddValue(JSValue value)
        //{
        //    if (HasValueOf(out var v))
        //        return v.AddValue(value);
        //    return base.AddValue(value);
        //}

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
            return prototypeChain?.GetMethod(key);
        }

        public override JSValue this[KeyString name] { 
            get => this.GetValue(GetInternalProperty(name)); 
            set {
                var start = this;
                var p = GetInternalProperty(name, true);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(new Arguments(this, value));
                        return;
                    }
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                    //return;
                }
                if(p.IsReadOnly)
                {
                    // Only in Strict Mode ..
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                }
                if (this.IsFrozen())
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                if(p.IsEmpty && !this.IsExtensible())
                    throw JSContext.Current.NewTypeError($"Cannot add property {name} to {this}");
                ref var ownProperties = ref this.GetOwnProperties();
                ownProperties[name.Key] = JSProperty.Property(name, value);
                PropertyChanged?.Invoke(this, (name.Key, uint.MaxValue, null));
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
                ref var elements = ref CreateElements();
                elements[name] = JSProperty.Property(value);
                PropertyChanged?.Invoke(this, (uint.MaxValue, name, null));
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
                symbols[name.Key.Key] = JSProperty.Property(value);
                PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, name));
            }
        }

        public JSValue DefineProperty(JSSymbol name, in JSProperty p)
        {
            var key = name.Key.Key;
            var old = symbols[key];
            if (!old.IsEmpty)
            {
                if (!old.IsConfigurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            // p.key = name.Key;
            symbols[key] = p.With(name.Key);
            PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, name));
            return JSUndefined.Value;
        }

        public JSValue DefineProperty(in KeyString name, in JSProperty p)
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
            // p.key = name;
            ownProperties[key] = p.With(name);
            PropertyChanged?.Invoke(this, (name.Key, uint.MaxValue, null));
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
            PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, null));
        }

        public override string ToString()
        {
            var px = GetMethod(KeyStrings.toString);
            if (px != null)
            {
                var v = px(new Arguments(this));
                if (v != this)
                {
                    return v.ToString();
                }
            }
            return "[object Object]";
        }

        // prevent recursive...
        public override string ToDetailString()
        {
            var all = this.GetAllEntries(false).Select((e) => $"{e.Key}: {e.Value?.ToString()}");
            return $"{{ {string.Join(", ", all)} }}";
        }

        public override double DoubleValue{
            get {
                try
                {
                    var fx = this[KeyStrings.valueOf];
                    if (fx.IsUndefined)
                        return NumberParser.CoerceToNumber(this.ToString());
                    var v = fx.InvokeFunction(new Arguments(this));
                    if (v == this)
                        return double.NaN;
                    return v.DoubleValue;
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return double.NaN;
                }
            }
        }

        public override int Length {
            get
            {
                try
                {
                    ref var ownp = ref this.ownProperties;
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
                }catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return -1;
                }
            }
            set {
                if (this.IsSealedOrFrozenOrNonExtensible())
                    throw JSContext.Current.NewTypeError($"Cannot modify property length of {this}");
                ref var ownp = ref GetOwnProperties();
                ownp[KeyStrings.length.Key] = JSProperty.Property(KeyStrings.length,new JSNumber(value));
                PropertyChanged?.Invoke(this, (KeyStrings.length.Key, uint.MaxValue, null));
            }
        }

        public override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
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
            JSFunction pget = null;
            JSFunction pset = null;
            JSValue pvalue = null;
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
                pget = get;
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
                pset = set;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                pvalue = value;
            }
            var pAttributes = pt;
            ref var elements = ref target.CreateElements();
            elements[key] = new JSProperty(KeyString.Empty, pget, pset, pvalue, pAttributes);
            if (target is JSArray array)
            {
                if (array._length <= key)
                    array._length = key + 1;
            }
            target.PropertyChanged?.Invoke(target, (uint.MaxValue, key , null));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InternalAddProperty(JSObject target, in KeyString key, JSValue pd)
        {
            JSFunction pget = null;
            JSFunction pset = null;
            JSValue pvalue = null;
            var value = pd[KeyStrings.value];
            var get = pd[KeyStrings.get] as JSFunction;
            var set = pd[KeyStrings.set] as JSFunction;
            var pt = JSPropertyAttributes.Empty;
            if (pd[KeyStrings.configurable].BooleanValue)
                pt |= JSPropertyAttributes.Configurable;
            if (pd[KeyStrings.enumerable].BooleanValue)
                pt |= JSPropertyAttributes.Enumerable;
            if (!pd[KeyStrings.writable].BooleanValue)
                pt |= JSPropertyAttributes.Readonly;
            if (get != null)
            {
                pt |= JSPropertyAttributes.Property;
                pget = get;
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
                pset = set;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                pvalue = value;
                pget = value as JSFunction;
            }
            var pAttributes = pt;
            ref var ownProperties = ref target.GetOwnProperties();
            ownProperties[key.Key] = new JSProperty(key, pget, pset, pvalue, pAttributes);
            target.PropertyChanged?.Invoke(target, (key.Key, uint.MaxValue, null));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InternalAddProperty(JSObject target, JSSymbol key, JSValue pd)
        {
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
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                get = value as JSFunction;
            }
            // p.Attributes = pt;
            // var symbols = target.symbols ?? (target.symbols = new CompactUInt32Trie<JSProperty>());
            target.symbols[key.Key.Key] = new JSProperty(key.Key, get, set, value, pt);
            target.PropertyChanged?.Invoke(target, (uint.MaxValue, uint.MaxValue, key));
        }


        public override JSValue Delete(KeyString key)
        {
            if (this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
            if (ownProperties.RemoveAt(key.Key))
            {
                PropertyChanged?.Invoke(this, (key.Key, uint.MaxValue, null));
                return JSBoolean.True;
            }
            return JSBoolean.True;
        }

        public override JSValue Delete(uint key)
        {
            if (this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot delete property {key} of {this}");
            if (elements.RemoveAt(key))
            {
                PropertyChanged?.Invoke(this, (uint.MaxValue, key, null));
                return JSBoolean.True;
            }
            return JSBoolean.True;
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
            throw JSContext.Current.NewTypeError($"{this} is not a function");
        }

        public override JSBoolean Less(JSValue value)
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

        public override JSBoolean LessOrEqual(JSValue value)
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

        public override JSBoolean Greater(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.value) > 0):
                        return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        public override JSBoolean GreaterOrEqual(JSValue value)
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
            return elements.TryGetValue(i, out value);
        }

        internal override bool TryGetElement(uint i, out JSValue value)
        {
            if (elements.TryGetValue(i, out var p)) {
                value = this.GetValue(p);
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Moves elements from `start` to `to`.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="to"></param>
        internal override void MoveElements(int start, int to)
        {
            ref var elements = ref CreateElements();

            var end = this.Length - 1;
            var diff = to - start;
            if (start > to)
            {

                for (uint i = (uint)start, j = (uint)to; i <= end; i++, j++)
                {
                    if (this.TryRemove(i, out var p))
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
                    if (this.TryRemove((uint)i, out var p))
                    {
                        elements[(uint)j] = p;
                    }
                }
                this.Length += diff;
            }
            PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, null));

        }

        /// <summary>
        /// Used in pop
        /// </summary>
        /// <param name="i"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        internal override bool TryRemove(uint i, out JSProperty p)
        {
            if (elements.TryRemove(i, out p))
            {
                PropertyChanged?.Invoke(this, (uint.MaxValue, i, null));
                return true;
            }
            if (prototypeChain != null)
                return prototypeChain.TryRemove(i, out p);
            return false;
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

        private readonly struct ElementEnumerator : IElementEnumerator
        {
            private readonly JSObject @object;
            readonly IEnumerator<(uint Key, JSProperty Value)> en;
            public ElementEnumerator(JSObject @object)
            {
                this.en = @object.elements.AllValues().GetEnumerator();
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

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                if (en?.MoveNext() ?? false)
                {
                    var (Key, Value) = en.Current;
                    value = @object.GetValue(Value);
                    return true;
                }
                value = @default;
                return false;
            }

        }

    }
}
