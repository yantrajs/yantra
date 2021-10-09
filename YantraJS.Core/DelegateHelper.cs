using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace YantraJS.Core
{
    public static class DelegateHelper
    {

        public static JSFunctionDelegate CreateSetter<T, TInput>(Action<T, TInput> fx)
        {
            return (in Arguments a) => {
                var input = a.Get1();
                var t = (T)a.This.ForceConvert(typeof(T));
                fx(t, (TInput) input.ForceConvert(typeof(TInput)));
                return input;
            };
        }

        public static JSFunctionDelegate CreateJSValueSetter<T>(Action<T, JSValue> fx)
        {
            return (in Arguments a) => {
                var input = a.Get1();
                var t = (T)a.This.ForceConvert(typeof(T));
                fx(t, input);
                return input;
            };
        }

        public static JSFunctionDelegate CreateGetter<T, TRet>(Func<T, TRet> fx)
        {
            return (in Arguments a) => {
                var t = (T)a.This.ForceConvert(typeof(T));
                return fx(t).Marshal();
            };
        }

        public static JSFunctionDelegate CreateJSValueGetter<T>(Func<T, JSValue> fx)
        {
            return (in Arguments a) => {
                var t = (T)a.This.ForceConvert(typeof(T));
                return fx(t);
            };
        }

        private static Type type = typeof(DelegateHelper);
        private static MethodInfo createGetter = type.GetMethod(nameof(CreateGetter));
        private static MethodInfo createJSValueGetter = type.GetMethod(nameof(CreateJSValueGetter));
        private static MethodInfo createSetter = type.GetMethod(nameof(CreateSetter));
        private static MethodInfo createJSValueSetter = type.GetMethod(nameof(CreateJSValueSetter));

        public static JSFunctionDelegate CreatePropertySetter(PropertyInfo property)
        {
            var dt = typeof(Action<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var d = Delegate.CreateDelegate(dt, property.SetMethod);
            if (typeof(JSValue).IsAssignableFrom(property.PropertyType))
                return (JSFunctionDelegate)createJSValueSetter.MakeGenericMethod(property.DeclaringType)
                    .Invoke(null, new object[] { d });
            return (JSFunctionDelegate)createSetter.MakeGenericMethod(property.DeclaringType, property.PropertyType)
                .Invoke(null, new object[] { d });
        }

        public static JSFunctionDelegate CreatePropertyGetter(PropertyInfo property)
        {
            var dt = typeof(Func<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var d = Delegate.CreateDelegate(dt, property.GetMethod);
            if (typeof(JSValue).IsAssignableFrom(property.PropertyType))
                return (JSFunctionDelegate)createJSValueGetter.MakeGenericMethod(property.DeclaringType)
                    .Invoke(null, new object[] { d });
            return (JSFunctionDelegate)createGetter.MakeGenericMethod(property.DeclaringType, property.PropertyType)
                .Invoke(null, new object[] { d });
        }

        public static JSFunctionDelegate Method<T>(StaticDelegate<T> fx)
            where T: class
        {
            return (in Arguments a) => {
                var @this = a.This.ForceConvert(typeof(T)) as T
                    ?? throw JSContext.Current.NewTypeError($"this is not of type {typeof(T).Name}");
                return fx(@this, in a);
            };
        }

        private static MethodInfo method = type.GetMethod(nameof(Method));


        public delegate JSValue StaticDelegate<T>(T target, in Arguments a);
        public static JSFunctionDelegate CreateMethod(MethodInfo method)
        {
            var dt = typeof(StaticDelegate<>).MakeGenericType(method.DeclaringType);
            var d = Delegate.CreateDelegate(dt, method);
            return (JSFunctionDelegate)method.MakeGenericMethod(method.DeclaringType).Invoke(null, new object[] { d });
        }
    }
}
