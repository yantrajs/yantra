using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using YantraJS.Core;

namespace YantraJS
{

    public static class JSObjectExtensions
    {


        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddSetter(JSObject target, KeyString key, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            ref var pr = ref target.GetOwnProperties();
            ref var existing = ref pr.Put(key.Key);
            var getter = existing.get;
            existing = new JSProperty(key, getter, setter, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddGetter(JSObject target, KeyString key, JSFunction getter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            ref var pr = ref target.GetOwnProperties();
            ref var existing = ref pr.Put(key.Key);
            var setter = existing.set;
            existing = new JSProperty(key, getter, setter, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddSetter(JSObject target, JSSymbol key, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            ref var pr = ref target.GetSymbols();
            ref var existing = ref pr.Put(key.Key);
            var getter = existing.get;
            existing = new JSProperty(key.Key, getter, setter, existing.value, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddGetter(JSObject target, JSSymbol key, JSFunction getter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            ref var pr = ref target.GetSymbols();
            ref var existing = ref pr.Put(key.Key);
            var setter = existing.set;
            existing = new JSProperty(key.Key, getter, setter, existing.value, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddSetter(JSObject target, uint key, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            ref var pr = ref target.GetElements(true);
            ref var existing = ref pr.Put(key);
            if (target is JSArray a)
                a._length = a._length > key ? a._length : key + 1;
            var getter = existing.get;
            existing = new JSProperty(key, getter, setter, existing.value, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddGetter(JSObject target, uint key, JSFunction getter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            ref var pr = ref target.GetElements(true);
            ref var existing = ref pr.Put(key);
            if (target is JSArray a)
                a._length = a._length > key ? a._length : key + 1;
            var setter = existing.set;
            existing = new JSProperty(key, getter, setter, existing.value, attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddSetter(JSObject target, JSValue key, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            var k = key.ToKey();
            switch (k.Type)
            {
                case KeyType.String:
                    FastAddSetter(target, k.KeyString, setter, attributes);
                    return;
                case KeyType.UInt:
                    FastAddSetter(target, k.Index, setter, attributes);
                    return;
                case KeyType.Symbol:
                    FastAddSetter(target, k.Symbol, setter, attributes);
                    return;
                default:
                    throw new NotSupportedException();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void FastAddGetter(JSObject target, JSValue key, JSFunction getter, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty)
        {
            var k = key.ToKey();
            switch (k.Type)
            {
                case KeyType.String:
                    FastAddGetter(target, k.KeyString, getter, attributes);
                    return;
                case KeyType.UInt:
                    FastAddGetter(target, k.Index, getter, attributes);
                    return;
                case KeyType.Symbol:
                    FastAddGetter(target, k.Symbol, getter, attributes);
                    return;
                default:
                    throw new NotSupportedException();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, in KeyString key, JSValue value, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            target.GetOwnProperties().Put(in key, value, attributes);
            return target;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, in KeyString key, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            ref var ownProperties = ref target.GetOwnProperties();
            ref var p = ref ownProperties.GetValue(key.Key);
            if (p.IsEmpty)
            {
                ownProperties.Put(key.Key) = JSProperty.Property(key, getter, setter, attributes);
                return target;
            }
            p = JSProperty.Property(key, getter ?? p.get, setter ?? p.set, attributes);
            return target;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, uint key, JSValue value, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            target.GetElements().Put(key) = JSProperty.Property(value, attributes);
            return target;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, uint key, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            ref var ownProperties = ref target.GetElements();
            ref var p = ref ownProperties.Get(key);
            if (p.IsEmpty)
            {
                ownProperties.Put(key) = JSProperty.Property(getter, setter, attributes);
                return target;
            }
            p = JSProperty.Property(getter ?? p.get, setter ?? p.set, attributes);
            return target;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, JSSymbol key, JSValue value, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            target.GetSymbols().Put(key.Key) = JSProperty.Property(value, attributes);
            return target;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, JSSymbol key, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            ref var ownProperties = ref target.GetSymbols();
            ref var p = ref ownProperties.GetRefOrDefault(key.Key, ref JSProperty.Empty);
            if (p.IsEmpty)
            {
                ownProperties.Put(key.Key) = JSProperty.Property(getter, setter, attributes);
                return target;
            }
            p = JSProperty.Property(getter ?? p.get, setter ?? p.set, attributes);
            return target;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, JSValue name, JSValue value, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            var key = name.ToKey();
            if (key.IsSymbol)
            {
                return AddProperty(target, key.Symbol, value, attributes);
            }
            if (key.IsUInt)
            {
                return target.AddProperty(key.Index, value, attributes);
            }
            return AddProperty(target, key.KeyString, value,attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSObject AddProperty(this JSObject target, JSValue name, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            var key = name.ToKey();
            if (key.IsSymbol)
            {
                return AddProperty(target, key.Symbol, getter, setter, attributes);
            }
            if (key.IsUInt)
            {
                return target.AddProperty(key.Index, getter, setter, attributes);
            }
            return AddProperty(target, key.KeyString, getter, setter, attributes);
        }
    }

}
