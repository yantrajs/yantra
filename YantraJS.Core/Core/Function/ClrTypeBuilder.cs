using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace YantraJS.Core.Clr
{
    internal static class ClrTypeBuilder
    {
        private static MethodInfo SetStaticValueMethod =
            typeof(ClrTypeBuilder).GetMethod(nameof(SetStaticValue));

        private static MethodInfo SetValueMethod =
            typeof(ClrTypeBuilder).GetMethod(nameof(SetValue));

        private static MethodInfo GetStaticValueMethod =
            typeof(ClrTypeBuilder).GetMethod(nameof(GetStaticValue));

        private static MethodInfo GetValueMethod =
            typeof(ClrTypeBuilder).GetMethod(nameof(GetValue));

        public static JSFunction GetValue<TOwner, TValue>(PropertyInfo property)
        {
            var name = $"get {property.Name.ToCamelCase()}";
            var getter = (Func<TOwner, TValue>)property.GetMethod.CreateDelegate(typeof(Func<TOwner, TValue>));
            var del = ClrProxy.GetDelegate<TValue>();
            return new JSFunction((in Arguments a) => {
                var owner = a.This;
                var value = getter((TOwner)owner.ForceConvert(typeof(TOwner)));
                return del(value);
            }, name);
        }

        public static JSFunction GetStaticValue<TValue>(PropertyInfo property)
        {
            var name = $"get {property.Name.ToCamelCase()}";
            var getter = (Func<TValue>)property.GetMethod.CreateDelegate(typeof(Func<TValue>));
            var del = ClrProxy.GetDelegate<TValue>();
            return new JSFunction( (in Arguments a) => {
                var value = getter();
                return del(value);
            }, name);
        }

        public static JSFunction SetValue<TOwner, TValue>(PropertyInfo property)
        {
            var name = $"set {property.Name.ToCamelCase()}";
            var setter = (Action<TOwner, TValue>)property.SetMethod.CreateDelegate(typeof(Action<TOwner, TValue>));
            return new JSFunction((in Arguments a) =>
            {
                var owner = a.This;
                var value = a.Get1();
                setter(
                    (TOwner)owner.ForceConvert(typeof(TOwner)),
                    (TValue)value.ForceConvert(typeof(TValue)));
                return JSUndefined.Value;
            }, name);
        }

        public static JSFunction SetStaticValue<TValue>(PropertyInfo property)
        {
            var name = $"set {property.Name.ToCamelCase()}";
            var setter = (Action<TValue>)property.SetMethod.CreateDelegate(typeof(Action<TValue>));
            return new JSFunction((in Arguments a) =>
            {
                var value = a.Get1();
                setter(
                    (TValue)value.ForceConvert(typeof(TValue)));
                return JSUndefined.Value;
            }, name);
        }

        internal static JSFunction GeneratePropertySetter(this PropertyInfo property)
        {
            if (property.SetMethod.IsStatic)
            {
                return (JSFunction) SetStaticValueMethod
                    .MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property });
            }
            return (JSFunction)SetValueMethod
                .MakeGenericMethod(property.DeclaringType, property.PropertyType).Invoke(null, new object[] { property });
        }

        internal static JSFunction GeneratePropertyGetter(this PropertyInfo property)
        {
            if (property.GetMethod.IsStatic)
            {
                return (JSFunction)GetStaticValueMethod
                    .MakeGenericMethod(property.PropertyType).Invoke(null, new object[] { property });
            }
            return (JSFunction)GetValueMethod
                .MakeGenericMethod(property.DeclaringType, property.PropertyType).Invoke(null, new object[] { property });
        }
    }
}
