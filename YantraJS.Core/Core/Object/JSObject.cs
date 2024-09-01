using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Yantra.Core;
using YantraJS.Core.Core;
using YantraJS.Core.Core.Generator;
using YantraJS.Core.Core.Storage;
using YantraJS.Core.Enumerators;
using YantraJS.Extensions;
using YantraJS.Utils;

namespace YantraJS.Core
{

    internal delegate void PropertyChangedEventHandler(JSObject sender, (uint keyString, uint index, JSSymbol symbol) index);

    // [JSRuntime(typeof(JSObjectStatic), typeof(JSObjectPrototype))]
    [JSBaseClass("Object")]
    [JSFunctionGenerator("Object", Register = false)]

    public partial class JSObject : JSValue
    {
        private JSPrototype currentPrototype;
        protected bool HasIterator = false;

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
        private SAUint32Map<JSProperty> symbols;
        private long? uid;

        private static long NextID = 0;
        internal long UniqueID => uid ??= Interlocked.Increment(ref NextID);

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
            //if (ownProperties.IsEmpty && create)
            //    ownProperties = new PropertySequence(4);
            return ref ownProperties;
        }


        public override JSValue GetOwnPropertyDescriptor(JSValue name)
        {
            var key = name.ToKey(false);
            switch(key.Type)
            {
                case KeyType.String:
                    if(ownProperties.TryGetValue(key.KeyString.Key, out var p))
                        return p.ToJSValue();
                    return JSUndefined.Value;
                case KeyType.UInt:
                    if (elements.TryGetValue(key.Index, out var p1))
                        return p1.ToJSValue();
                    return JSUndefined.Value;
                case KeyType.Symbol:
                    if (symbols.TryGetValue(key.Symbol.Key, out var p3))
                        return p3.ToJSValue();
                    return JSUndefined.Value;
            }
            return JSUndefined.Value;
        }

        public override JSValue GetOwnProperty(in KeyString name)
        {
            ref var p = ref ownProperties.GetValue(name.Key);
            return this.GetValue(p);
        }

        public override JSValue GetOwnProperty(JSSymbol name)
        {
            ref var p = ref symbols.GetRefOrDefault(name.Key, ref JSProperty.Empty);
            return this.GetValue(p);
        }

        public override JSValue GetOwnProperty(uint name)
        {
            ref var p = ref elements.Get(name);
            return this.GetValue(p);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ElementArray GetElements(bool create = true)
        {
            //if (elements.IsNull && create)
            //    elements = new UInt32Map<JSProperty>();
            return ref elements;
        }

        public ref SAUint32Map<JSProperty> GetSymbols()
        {
            return ref symbols;
        }

        internal void AllocateElements(uint size)
        {
            size = size > 1024 ? 1024 : size;
            elements.Resize(size);
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

        public JSObject() : base((JSObject)null)
        {
            
        }

        public virtual IEnumerable<(string Key, JSValue value)> Entries
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
                var en = new PropertySequence.ValueEnumerator(this, false);
                while(en.MoveNext(out var value, out var key))
                {
                    yield return (KeyStrings.GetNameString(key.Key).Value, value);
                }
                //for(int i = 0; i< ownProperties.properties.Length; i++)
                //{
                //    var p = ownProperties.properties[i];
                //    JSValue v = null;
                //    try {
                //        v = this.GetValue(p);
                //    } catch (Exception ex)
                //    {
                //        System.Diagnostics.Debug.WriteLine(ex);
                //    }
                //    yield return ( KeyStrings.GetNameString(p.key).Value , v);
                //}
            }
        }

        //public JSObject(params JSProperty[] entries) : this(JSContext.Current?.ObjectPrototype)
        //{
        //    ownProperties = new PropertySequence(4);
        //    foreach (var p in entries)
        //    {
        //        ownProperties.Put(p.key.Key) = p;
        //    }
        //}

        public JSObject(IEnumerable<JSProperty> entries) : this(JSContext.Current?.ObjectPrototype)
        {
            // ownProperties = new PropertySequence(4);
            foreach (var p in entries)
            {
                ownProperties.Put(p.key) = p;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject NewWithProperties()
        {
            var o = new JSObject
            {
                // ownProperties = new PropertySequence(4)
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
                // ownProperties = new PropertySequence(4),
                // elements = new ElementArray(4)
            };
            return o;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddValue(uint index, JSValue value, JSPropertyAttributes attributes)
        {
            elements.Put(index, value, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddProperty(uint index, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes)
        {
            elements.Put(index) = new JSProperty(index, getter, setter, getter, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddValue(KeyString key, JSValue value, JSPropertyAttributes attributes)
        {
            ref var pr = ref GetOwnProperties(true);
            pr.Put(key.Key) = new JSProperty(key.Key, value, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddProperty(KeyString key, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes)
        {
            ref var pr = ref GetOwnProperties(true);
            pr.Put(key.Key) = new JSProperty(key,getter, setter, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddValue(JSSymbol key, JSValue value, JSPropertyAttributes attributes)
        {
            ref var pr = ref GetSymbols();
            pr.Put(key.Key) = new JSProperty(key.Key, value, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddProperty(JSSymbol key, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes)
        {
            ref var pr = ref GetSymbols();
            pr.Put(key.Key) = new JSProperty(key.Key, getter, setter, getter, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddValue(JSValue key, JSValue value, JSPropertyAttributes attributes)
        {
            var k = key.ToKey(true);
            switch(k.Type)
            {
                case KeyType.String:
                    FastAddValue(k.KeyString, value, attributes);
                    return;
                case KeyType.UInt:
                    FastAddValue(k.Index, value, attributes);
                    return;
                default:
                    FastAddValue(k.Symbol, value, attributes);
                    return;
            }
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FastAddProperty(JSValue key, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes)
        {
            var k = key.ToKey(true);
            switch (k.Type)
            {
                case KeyType.String:
                    FastAddProperty(k.KeyString, getter, setter, attributes);
                    return;
                case KeyType.UInt:
                    FastAddProperty(k.Index, getter,setter, attributes);
                    return;
                default:
                    FastAddProperty(k.Symbol, getter, setter, attributes);
                    return;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void FastAddRange(JSValue value)
        {
            if (!(value is JSObject target))
                return;
            var pe = target.ownProperties.GetEnumerator();
            while (pe.MoveNext(out var key, out var val) && !val.IsEmpty)
            {
                this.ownProperties.Put(key.Key) = val.IsValue
                    ? JSProperty.Property(val.value)
                    : JSProperty.Property(target.GetValue(val));
            }
            var en = target.elements.Length;
            for(uint i = 0; i< en; i++)
            {
                if (target.elements.TryGetValue(i, out var p) && !p.IsEmpty)
                {
                    this.elements.Put(i) = p.IsValue
                        ? JSProperty.Property(p.value)
                        : JSProperty.Property(target.GetValue(p));
                }
            }
            foreach(var symbol in target.symbols.All)
            {
                var key = symbol.Key;
                var sv = symbol.Value;
                if (sv.IsEmpty)
                {
                    continue;
                }
                this.symbols.Put(key) = sv.IsValue
                    ? JSProperty.Property(sv.value)
                    : JSProperty.Property(target.GetValue(sv));
            }
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSObject Merge(JSValue value)
        {
            if (!(value is JSObject target))
                return this;
            var pe = new PropertyEnumerator(target, true, false);
            while(pe.MoveNext(out var key, out var val))
            {
                this[key] = val;
            }
            var en = new ElementEnumerator(target);
            while(en.MoveNext(out var hasValue, out var val, out var index))
            {
                if (hasValue)
                {
                    this[index] = val;
                }
            }
            return this;
        }

        internal override PropertyKey ToKey(bool create = true)
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
            if (symbols.TryGetValue(key.Key, out var r))
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
            get => this.GetValue(name, this);
            set => SetValue(name, value, null, true);
        }

        internal protected override bool SetValue(KeyString name, JSValue value, JSValue receiver, bool throwError = true)
        {
            var start = this;
            var p = GetInternalProperty(name, true);
            if (p.IsProperty)
            {
                if (p.set != null)
                {
                    p.set.f(new Arguments(receiver ?? this, value));
                    return true;
                }
                if (throwError)
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this} which has only a getter");
                return false;
            }
            if (p.IsReadOnly)
            {
                if (throwError)
                {
                    // Only in Strict Mode ..
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                }
                return false;
            }
            if (this.IsFrozen())
            {
                if(throwError)
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                return false;
            }
            if (p.IsEmpty && !this.IsExtensible())
            {
                if (throwError)
                {
                    throw JSContext.Current.NewTypeError($"Cannot add property {name} to {this}");
                }
                return false;
            }
            ref var ownProperties = ref this.GetOwnProperties();
            ownProperties.Put(name, value, !p.IsEmpty ? p.Attributes : JSPropertyAttributes.EnumerableConfigurableValue);
            PropertyChanged?.Invoke(this, (name.Key, uint.MaxValue, null));
            return true;
        }

        public override JSValue this[uint name]
        {
            get => GetValue(name, this);
            set => SetValue(name, value, this, true);
        }

        internal protected override bool SetValue(uint name, JSValue value, JSValue receiver, bool throwError = true)
        {
            var p = GetInternalProperty(name);
            if (p.IsProperty)
            {
                if (p.set != null)
                {
                    p.set.f(new Arguments(receiver ?? this, value));
                    return true;
                }
                return false;
            }
            if (this.IsFrozen())
            {
                if(throwError)
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                return false;
            }
            ref var elements = ref CreateElements();
            elements.Put(name, value);
            PropertyChanged?.Invoke(this, (uint.MaxValue, name, null));
            return true;
        }

        public override JSValue this[JSSymbol name]
        {
            get => GetValue(name, this);
            set => SetValue(name, value, null, true);
        }

        internal protected override bool SetValue(JSSymbol name, JSValue value, JSValue receiver, bool throwError = true)
        {
            if (name == JSSymbol.iterator)
            {
                HasIterator = true;
            }
            var p = GetInternalProperty(name);
            if (p.IsProperty)
            {
                if (p.set != null)
                {
                    p.set.f(new Arguments(receiver ?? this, value));
                    return true;
                }
                return false;
            }
            if (this.IsFrozen())
            {
                if (throwError)
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                return false;
            }
            symbols.Put(name.Key) = JSProperty.Property(value);
            PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, name));
            return true;
        }

        internal protected override JSValue GetValue(JSSymbol key, JSValue receiver, bool throwError = true)
        {
            ref var p = ref symbols.GetRefOrDefault(key.Key, ref JSProperty.Empty);
            if (!p.IsEmpty)
            {
                return (receiver ?? this).GetValue(p);
            }
            return base.GetValue(key, receiver, throwError);
        }

        internal protected override JSValue GetValue(KeyString key, JSValue receiver, bool throwError = true)
        {
            ref var p = ref ownProperties.GetValue(key.Key);
            if (!p.IsEmpty)
            {
                return (receiver ?? this).GetValue(p);
            }
            return base.GetValue(key, receiver, throwError);
        }

        internal protected override JSValue GetValue(uint key, JSValue receiver, bool throwError = true)
        {
            ref var p = ref elements.Get(key);
            if (!p.IsEmpty)
            {
                if (p.IsValue)
                    return p.value;
                return p.get.InvokeFunction(new Arguments((receiver ?? this)));
            }
            return base.GetValue(key, receiver, throwError);
        }

        public virtual JSValue DefineProperty(JSValue key, JSObject propertyDescription)
        {
            var k = key.ToKey();
            switch (k.Type)
            {
                case KeyType.Empty:
                    return JSBoolean.False;
                case KeyType.UInt:
                    return DefineProperty(k.Index, propertyDescription);
                case KeyType.String:
                    return DefineProperty(k.KeyString, propertyDescription);
                case KeyType.Symbol:
                    return DefineProperty(k.Symbol, propertyDescription);
            }
            return JSBoolean.False;
        }

        public JSValue DefineProperty(JSSymbol name, JSObject pd)
        {
            var key = name.Key;
            var old = symbols[key];
            if (!old.IsEmpty)
            {
                if (!old.IsConfigurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            symbols.Put(key) = pd.ToProperty(key);
            PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, name));
            return JSUndefined.Value;
        }

        public JSValue DefineProperty(uint key, JSObject pd)
        {
            ref var elements = ref GetElements(true);
            var old = elements[key];
            if (!old.IsEmpty)
            {
                if (!old.IsConfigurable)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            // p.key = name;
            elements.Put(key) = pd.ToProperty(key);
            if (this is JSArray array)
            {
                if (array._length <= key)
                    array._length = key + 1;
            }
            PropertyChanged?.Invoke(this, (uint.MaxValue, key, null));
            return JSUndefined.Value;
        }

        public JSValue DefineProperty(in KeyString name, JSObject pd)
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
            ownProperties.Put(key) = pd.ToProperty(key);
            PropertyChanged?.Invoke(this, (name.Key, uint.MaxValue, null));
            return JSUndefined.Value;
        }

        //public void DefineProperties(params JSProperty[] list)
        //{
        //    ref var ownProperties = ref GetOwnProperties();
        //    foreach (var p in list)
        //    {
        //        var key = p.key;
        //        ref var old = ref ownProperties.GetValue(key);
        //        if (!old.IsEmpty)
        //        {
        //            if (!old.IsConfigurable)
        //            {
        //                throw new UnauthorizedAccessException();
        //            }
        //        }
        //        ownProperties.Put(key) = p;
        //    }
        //    PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, null));
        //}

        private bool toStringCalled = false;
        public override string ToString()
        {
            var px = GetMethod(KeyStrings.toString);
            if (px != null)
            {
                if (toStringCalled)
                {
                    return "Stack overflow";
                }
                toStringCalled = true;
                var v = px(new Arguments(this));
                if (v != this)
                {
                    toStringCalled = false;
                    return v.ToString();
                }
            }
            return "[object Object]";
        }

        // prevent recursive...
        public override string ToDetailString()
        {
            var sb = new StringBuilder();
            bool first = true;
            sb.Append('{');
            foreach(var (Key,Value) in this.GetAllEntries(false))
            {                
                if (Value == this)
                    continue;
                if (!first)
                    sb.Append(", ");
                first = false;
                sb.Append($"{Key}: {Value?.ToDetailString()}");
            }
            sb.Append('}');
            return sb.ToString();
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
                    //var px = this.GetInternalProperty(KeyStrings.length);
                    //if (!px.IsEmpty)
                    //{
                    //    var n = this.GetValue(px);
                    //    var nValue = ((uint)n.DoubleValue) >> 0;
                    //    return (int)nValue;
                    //}
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
                ownp.Put(KeyStrings.length,new JSNumber(value));
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

        internal JSProperty ToProperty(uint key)
        {
            JSFunction pget = null;
            JSFunction pset = null;
            JSValue pvalue = null;
            var value = this[KeyStrings.value];
            var get = this[KeyStrings.get] as JSFunction;
            var set = this[KeyStrings.set] as JSFunction;
            var pt = JSPropertyAttributes.Empty;
            if (this[KeyStrings.configurable].BooleanValue)
                pt |= JSPropertyAttributes.Configurable;
            if (this[KeyStrings.enumerable].BooleanValue)
                pt |= JSPropertyAttributes.Enumerable;
            if (!this[KeyStrings.writable].BooleanValue)
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
            return new JSProperty(key, pget, pset, pvalue, pAttributes);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void InternalAddProperty(JSObject target, uint key, JSObject pd)
        //{
        //    ref var elements = ref target.CreateElements();
        //    elements.Put(key) = pd.ToProperty();
        //    if (target is JSArray array)
        //    {
        //        if (array._length <= key)
        //            array._length = key + 1;
        //    }
        //    target.PropertyChanged?.Invoke(target, (uint.MaxValue, key , null));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void InternalAddProperty(JSObject target, in KeyString key, JSObject pd)
        //{
        //    ref var ownProperties = ref target.GetOwnProperties();
        //    ownProperties.Put(key.Key) = pd.ToProperty();
        //    target.PropertyChanged?.Invoke(target, (key.Key, uint.MaxValue, null));
        //}


        public override JSValue Delete(in KeyString key)
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
            ref var element = ref elements.Get(key);
            //if (!element.IsEmpty)
            //{
            //    PropertyChanged?.Invoke(this, (uint.MaxValue, key, null));
            //    element = JSProperty.Empty;
            //    return JSBoolean.True;
            //}
            if (elements.RemoveAt(key))
            {
                PropertyChanged?.Invoke(this, (uint.MaxValue, key, null));
                return JSBoolean.True;
            }
            return JSBoolean.True;
        }

        public override JSValue Delete(JSSymbol symbol)
        {
            if (this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot delete property {symbol} of {this}");
            if (symbols.RemoveAt(symbol.Key))
            {
                PropertyChanged?.Invoke(this, (uint.MaxValue, uint.MaxValue, symbol));
                return JSBoolean.True;
            }
            return JSBoolean.True;
        }

        public override bool Equals(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return true;
            if (value is JSString str)
                if (str.value.Equals(this.ToString()))
                    return true;
            if (DoubleValue == value.DoubleValue)
                return true;
            return false;
        }

        public override bool EqualsLiteral(double value)
        {
            return DoubleValue == value;
        }

        public override bool EqualsLiteral(string value)
        {
            return this.ToString() == value;
        }

        public override bool StrictEquals(JSValue value)
        {
            return Object.ReferenceEquals(this, value);
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this} is not a function");
        }

        public override bool Less(JSValue value)
        {
            switch(value)
            {
                case JSString strValue:
                    if (this.ToString().CompareTo(strValue.ToString()) < 0)
                        return true;
                    break;
            }
            return false;
        }

        public override bool LessOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return true;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.ToString()) <= 0):
                        return true;
            }
            return false;
        }

        public override bool Greater(JSValue value)
        {
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.ToString()) > 0):
                        return true;
            }
            return false;
        }

        public override bool GreaterOrEqual(JSValue value)
        {
            if (Object.ReferenceEquals(this, value))
                return true;
            switch (value)
            {
                case JSString strValue
                    when (this.ToString().CompareTo(strValue.ToString()) >= 0):
                        return true;
            }
            return false;
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
                        elements.Put(j) = p;
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
                        elements.Put((uint)j) = p;
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

        public override IElementEnumerator GetElementEnumerator()
        {
            if (this.HasIterator)
            {
                var v = this.GetValue(this.symbols[JSSymbol.iterator.Key]);
                return new JSIterator(v.InvokeFunction(new Arguments(this)));
            }
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

            public JSValue NextOrDefault(JSValue @default)
            {
                if (en?.MoveNext() ?? false)
                {
                    var (Key, Value) = en.Current;
                    return @object.GetValue(Value);
                }
                return @default;
            }
        }

    }
}
