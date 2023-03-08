using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace YantraJS.Core
{
    public static class DelegateHelper
    {

        public static JSFunctionDelegate CreateSetter<T, TInput>(MethodInfo m)
        {
            var fx = m.CreateDelegate<Action<T,TInput>>();
            return (in Arguments a) => {
                var input = a.Get1();
                var t = a.This.ToFastClrValue<T>();
                fx(t, (TInput) input.ForceConvert(typeof(TInput)));
                return input;
            };
        }

        public static JSFunctionDelegate CreateJSValueSetter<T>(MethodInfo m)
        {
            var fx = m.CreateDelegate<Action<T,JSValue>> ();
            return (in Arguments a) => {
                var input = a.Get1();
                var t = (T)a.This.ForceConvert(typeof(T));
                fx(t, input);
                return input;
            };
        }

        public static JSFunctionDelegate CreateGetter<T, TRet>(MethodInfo m)
        {
            var fx = m.CreateDelegate<Func<T, TRet>>();
            return (in Arguments a) => {
                var t = (T)a.This.ForceConvert(typeof(T));
                return fx(t).Marshal();
            };
        }

        public static JSFunctionDelegate CreateJSValueGetter<T>(MethodInfo m)
        {
            var fx = m.CreateDelegate<Func<T, JSValue>>();
            return (in Arguments a) => {
                var t = (T)a.This.ForceConvert(typeof(T));
                return fx(t);
            };
        }

        //private static Type type = typeof(DelegateHelper);
        //private static MethodInfo createGetter = type.GetMethod(nameof(CreateGetter));
        //private static MethodInfo createJSValueGetter = type.GetMethod(nameof(CreateJSValueGetter));
        //private static MethodInfo createSetter = type.GetMethod(nameof(CreateSetter));
        //private static MethodInfo createJSValueSetter = type.GetMethod(nameof(CreateJSValueSetter));

        public static JSFunctionDelegate CreatePropertySetter(PropertyInfo property)
        {
            //var dt = typeof(Action<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            //var d = (Action<object,JSValue>)Delegate.CreateDelegate(dt, property.SetMethod);
            if (typeof(JSValue).IsAssignableFrom(property.PropertyType))
            {
                //return (JSFunctionDelegate)createJSValueSetter.MakeGenericMethod(property.DeclaringType)
                //    .Invoke(null, new object[] { d });
                return Generic.InvokeAs(property.DeclaringType, CreateJSValueSetter<object>, property.SetMethod);
            }
            //return (JSFunctionDelegate)createSetter.MakeGenericMethod(property.DeclaringType, property.PropertyType)
            //    .Invoke(null, new object[] { d });
            return Generic.InvokeAs(property.DeclaringType, property.PropertyType, CreateSetter<object, object>, property.SetMethod);
        }

        public static JSFunctionDelegate CreatePropertyGetter(PropertyInfo property)
        {
            //var dt = typeof(Func<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            //var d = Delegate.CreateDelegate(dt, property.GetMethod);
            if (typeof(JSValue).IsAssignableFrom(property.PropertyType))
            {
                //return (JSFunctionDelegate)createJSValueGetter.MakeGenericMethod(property.DeclaringType)
                //    .Invoke(null, new object[] { d });
                return Generic.InvokeAs(property.DeclaringType, CreateJSValueGetter<object>, property.GetMethod);
            }
            //return (JSFunctionDelegate)createGetter.MakeGenericMethod(property.DeclaringType, property.PropertyType)
            //    .Invoke(null, new object[] { d });
            return Generic.InvokeAs(property.DeclaringType, property.PropertyType, CreateGetter<object,object>, property.GetMethod);
        }

        public static JSFunctionDelegate Method<T>(MethodInfo m)
            where T: class
        {
            var fx = m.CreateDelegate<StaticDelegate<T>>();
            return (in Arguments a) => {
                var @this = a.This.ForceConvert(typeof(T)) as T
                    ?? throw JSContext.Current.NewTypeError($"this is not of type {typeof(T).Name}");
                return fx(@this, in a);
            };
        }

        // private static MethodInfo method = type.GetMethod(nameof(Method));


        public delegate JSValue StaticDelegate<T>(T target, in Arguments a);
        public static JSFunctionDelegate CreateMethod(MethodInfo method)
        {
            //var dt = typeof(StaticDelegate<>).MakeGenericType(method.DeclaringType);
            //var d = Delegate.CreateDelegate(dt, method);
            //return (JSFunctionDelegate)method.MakeGenericMethod(method.DeclaringType).Invoke(null, new object[] { d });
            return Generic.InvokeAs(method.DeclaringType, Method<object>, method);
        }
    }
}
