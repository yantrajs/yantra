using System;
using System.Collections.Generic;
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
        Deleted = 64,

        // shortcuts..
        EnumerableConfigurableValue = Value | Enumerable | Configurable,
        EnumerableConfigurableReadonlyValue = Value | Enumerable | Configurable | Readonly,
        ConfigurableValue = Value | Configurable,
        ConfigurableReadonlyValue = Value | Configurable | Readonly,

        EnumerableConfigurableProperty = Property | Enumerable | Configurable,
        EnumerableConfigurableReadonlyProperty = Property | Enumerable | Configurable | Readonly,
        ConfigurableProperty = Property | Configurable,
        ConfigurableReadonlyProperty = Property | Configurable | Readonly,

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JSProperty
    {
        public JSPropertyAttributes Attributes;

        public KeyString key;

        // this slot will be used for getting method as well...
        // to avoid casting at runtime...
        public JSFunction get;
        public JSFunction set;
        public JSValue value;

        
        
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Function(
            JSFunctionDelegate d, 
            JSPropertyAttributes attributes = JSPropertyAttributes.Value | JSPropertyAttributes.Configurable)
        {
            var fx = new JSFunction(d);
            return new JSProperty
            {
                value = fx,
                get = fx,
                Attributes = attributes
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            JSValue d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty
            {
                value = d,
                get = d as JSFunction,
                Attributes = attributes
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            JSFunction d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty
            {
                value = d,
                get = d,
                Attributes = attributes
            };
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            JSFunctionDelegate get, 
            JSFunctionDelegate set = null, 
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            return new JSProperty
            {
                get = new JSFunction(get),
                set = set != null ? new JSFunction(set) : null,
                Attributes = attributes
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Function(
            KeyString key,
            JSFunctionDelegate d,
            JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableValue, int length = 0)
        {
            var fx = new JSFunction(d, null, null, length);
            return new JSProperty
            {
                key = key,
                get = fx,
                value = fx,
                Attributes = attributes
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            KeyString key,
            JSValue d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty
            {
                key = key,
                value = d,
                get = d as JSFunction,
                Attributes = attributes
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            KeyString key,
            JSFunction d,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            return new JSProperty
            {
                key = key,
                value = d,
                get = d,
                Attributes = attributes
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(
            KeyString key,
            JSFunctionDelegate get,
            JSFunctionDelegate set = null,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            return new JSProperty
            {
                key = key,
                get = new JSFunction(get),
                set = set != null ? new JSFunction(set) : null,
                Attributes = attributes
            };
        }

    }
}
