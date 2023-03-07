using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YantraJS
{
    public static class Generic
    {
        private static ConcurrentDictionary<(Type type1, Type type2, MethodInfo method), object> cache
            = new ConcurrentDictionary<(Type, Type, MethodInfo), object>();

        private static T CreateTypedDelegate<T>(this MethodInfo method)
            where T : Delegate
        {
            return (T)method.CreateDelegate(typeof(T));
        }

        private static T TypedGet<T>(
            (Type, Type, MethodInfo) key,
            Func<(Type type1, Type type2, MethodInfo method), T> create)
        {
            return (T)cache.GetOrAdd(key, (x) => create(x));
        }

        public static T InvokeAs<T>(Type type, Func<T> fx)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<T>>());
            return method();
        }

        public static T InvokeAs<T1, T>(Type type, Func<T1, T> fx, T1 p1)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<T1, T>>());
            return method(p1);
        }

        public static T InvokeAs<T1, T2, T>(Type type, Func<T1, T2, T> fx, T1 p1, T2 p2)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<T1, T2, T>>());
            return method(p1, p2);
        }
        public static T InvokeAs<T1, T2, T3, T>(Type type, Func<T1, T2, T3, T> fx, T1 p1, T2 p2, T3 p3)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<T1, T2, T3, T>>());
            return method(p1, p2, p3);
        }

        public static T InvokeAs<Target, T>(this Target target, Type type, Func<T> fx)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<Target, T>>());
            return method(target);
        }

        public static T InvokeAs<Target, T1, T>(this Target target, Type type, Func<T1, T> fx, T1 p1)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<Target, T1, T>>());
            return method(target, p1);
        }

        public static T InvokeAs<Target, T1, T2, T>(this Target target, Type type, Func<T1, T2, T> fx, T1 p1, T2 p2)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<Target, T1, T2, T>>());
            return method(target, p1, p2);
        }
        public static T InvokeAs<Target, T1, T2, T3, T>(this Target target, Type type, Func<T1, T2, T3, T> fx, T1 p1, T2 p2, T3 p3)
        {
            var method = TypedGet(
                    (type, type, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1)
                        .CreateTypedDelegate<Func<Target, T1, T2, T3, T>>());
            return method(target, p1, p2, p3);
        }

        public static T InvokeAs<Target, T>(this Target target, Type type, Type type2, Func<T> fx)
        {
            var method = TypedGet(
                    (type, type2, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1, k.type2)
                        .CreateTypedDelegate<Func<Target, T>>());
            return method(target);
        }

        public static T InvokeAs<Target, T1, T>(this Target target, Type type, Type type2, Func<T1, T> fx, T1 p1)
        {
            var method = TypedGet(
                    (type, type2, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1, k.type2)
                        .CreateTypedDelegate<Func<Target, T1, T>>());
            return method(target, p1);
        }

        public static T InvokeAs<Target, T1, T2, T>(this Target target, Type type, Type type2, Func<T1, T2, T> fx, T1 p1, T2 p2)
        {
            var method = TypedGet(
                    (type, type2, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1, k.type2)
                        .CreateTypedDelegate<Func<Target, T1, T2, T>>());
            return method(target, p1, p2);
        }
        public static T InvokeAs<Target, T1, T2, T3, T>(this Target target, Type type, Type type2, Func<T1, T2, T3, T> fx, T1 p1, T2 p2, T3 p3)
        {
            var method = TypedGet(
                    (type, type2, fx.Method),
                    (k) => k.method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(k.type1, k.type2)
                        .CreateTypedDelegate<Func<Target, T1, T2, T3, T>>());
            return method(target, p1, p2, p3);
        }
    }
}
