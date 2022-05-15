using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace YantraJS.Core
{
    public enum JSPropertyAttributes: byte
    {
        Empty = 0 ,
        Value = 1,
        Property = 2,
        Configurable = 8,
        Enumerable = 16,
        Readonly = 32,
        // Deleted = 64,

        // shortcuts..
        EnumerableConfigurableValue = Value | Enumerable | Configurable,
        EnumerableConfigurableReadonlyValue = Value | Enumerable | Configurable | Readonly,
        ConfigurableValue = Value | Configurable,
        ConfigurableReadonlyValue = Value | Configurable | Readonly,

        EnumerableConfigurableProperty = Property | Enumerable | Configurable,
        EnumerableConfigurableReadonlyProperty = Property | Enumerable | Configurable | Readonly,
        ConfigurableProperty = Property | Configurable,
        ConfigurableReadonlyProperty = Property | Configurable | Readonly,

        ReadonlyValue = Readonly | Value,
        ReadonlyProperty = Readonly | Property,

        EnumerableReadonlyValue = Enumerable | Readonly | Value,
        EnumerableReadonlyProperty = Enumerable | Readonly | Property
    }

    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{key}={get},{set},{value}")]
    public readonly struct JSProperty
    {

        public static JSProperty Empty = new JSProperty();

        // public static JSProperty Deleted = Empty.Delete();

        public readonly JSPropertyAttributes Attributes;

        public readonly uint key;

        // this slot will be used for getting method as well...
        // to avoid casting at runtime...
        public readonly JSFunction get;
        public readonly JSFunction set;
        public readonly JSValue value;

        internal JSProperty ToNotReadOnly()
        {
            return new JSProperty(key, get, set, value, Attributes & (~JSPropertyAttributes.Readonly));
        }

        //public JSProperty Delete()
        //{
        //    return new JSProperty(key, get, set, value, JSPropertyAttributes.Deleted);
        //}

        public JSProperty(
            in KeyString key,
            JSFunction get,
            JSFunction set,
            JSPropertyAttributes attributes)
        {
            this.key = key.Key;
            this.get = get;
            this.set = set;
            this.value = get;
            this.Attributes = attributes;
        }
        public JSProperty(
            uint key,
            JSFunction get,
            JSFunction set,
            JSValue value,
            JSPropertyAttributes attributes)
        {
            this.key = key;
            this.get = get ?? value as JSFunction;
            this.set = set;
            this.value = value;
            this.Attributes = attributes;
        }

        public JSProperty(
            in KeyString key,
            JSFunction get,
            JSFunction set,
            JSValue value,
            JSPropertyAttributes attributes)
        {
            this.key = key.Key;
            this.get = get;
            this.set = set;
            this.value = value;
            this.Attributes = attributes;
        }

        public JSProperty(
            uint key,
            JSValue get,
            JSPropertyAttributes attributes)
        {
            this.key = key;
            this.get = get as JSFunction;
            this.set = null;
            this.value = get;
            this.Attributes = attributes;
        }


        public JSProperty(
            in KeyString key,
            JSValue get,
            JSPropertyAttributes attributes)
        {
            this.key = key.Key;
            this.get = get as JSFunction;
            this.set = null;
            this.value = get;
            this.Attributes = attributes;
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Attributes == JSPropertyAttributes.Empty;
        }

        public bool IsConfigurable
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Attributes & JSPropertyAttributes.Configurable) > 0;
        }

        public bool IsEnumerable
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Attributes & JSPropertyAttributes.Enumerable) > 0;
        }

        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Attributes & JSPropertyAttributes.Readonly) > 0;
        }

        public bool IsValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Attributes & JSPropertyAttributes.Value) > 0;
        }

        public bool IsProperty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Attributes & JSPropertyAttributes.Property) > 0;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static JSProperty Function(
        //    JSFunctionDelegate d, 
        //    JSPropertyAttributes attributes = JSPropertyAttributes.Value | JSPropertyAttributes.Configurable)
        //{
        //    var fx = new JSFunction(d);
        //    return new JSProperty
        //    {
        //        value = fx,
        //        get = fx,
        //        Attributes = attributes
        //    };
        //}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            JSValue d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty(KeyString.Empty, d, attributes);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static JSProperty Property(
        //    JSFunction d,
        //    JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        //{
        //    return new JSProperty
        //    {
        //        value = d,
        //        get = d,
        //        Attributes = attributes
        //    };
        //}



        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static JSProperty Property(
        //    JSFunctionDelegate get, 
        //    JSFunctionDelegate set = null, 
        //    JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        //{
        //    return new JSProperty
        //    {
        //        get = new JSFunction(get),
        //        set = set != null ? new JSFunction(set) : null,
        //        Attributes = attributes
        //    };
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Function(
            in KeyString key,
            JSFunctionDelegate d,
            JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableValue, int length = 0)
        {
            var fx = new JSFunction(d, key.ToString(), null, length);
            return new JSProperty(key, fx, null, attributes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            uint key,
            JSValue d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty(key, d, attributes);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            in KeyString key,
            JSValue d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty(key, d, attributes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            in KeyString key,
            JSFunction d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty(key, d, attributes);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            in KeyString key,
            JSFunctionDelegate get,
            JSFunctionDelegate set = null,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            var fget = get == null ? null : new JSFunction(get, "get " + key.ToString());
            var fset = set == null ? null : new JSFunction(set, "set " + key.ToString());
            return new JSProperty(key, fget, fset, attributes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            in KeyString key,
            JSFunction get,
            JSFunction set = null,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            return new JSProperty(key, get, set, attributes);
        }
        public JSProperty With(in KeyString key)
        {
            return new JSProperty(key, get, set, value, Attributes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            JSFunction get,
            JSFunction set = null,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            return new JSProperty(KeyString.Empty, get, set, attributes);
        }

    }
}
