using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS
{

    public static class Generic
    {
        public static T CreateDelegate<T>(this MethodInfo m)
            where T : Delegate
        {
            return (T)m.CreateDelegate(typeof(T));
        }

        private static T CreateTypedDelegate<T>(
            this MethodInfo method)
            where T : Delegate
        {
            return (T)method.CreateDelegate(typeof(T));
        }

        private static T CreateTypedDelegate<T>(
            this MethodInfo method, params Type[] types)
            where T : Delegate
        {
            if (types.Length > 0)
            {
                method = method.GetGenericMethodDefinition().MakeGenericMethod(types);
            }
            return (T)method.CreateDelegate(typeof(T));
        }

        public static T InvokeAs<T>(Type type, Func<T> fx)
        {
            return fx.Method.CreateTypedDelegate<Func<T>>(type)();
        }

        public static T InvokeAs<T1, T>(Type type, Func<T1, T> fx, T1 p1)
        {
            return fx.Method.CreateTypedDelegate<Func<T1, T>>(type)(p1);
        }

        public static T InvokeAs<T1, T2, T>(Type type, Func<T1, T2, T> fx, T1 p1, T2 p2)
        {
            return fx.Method.CreateTypedDelegate<Func<T1, T2, T>>(type)(p1, p2);
        }
        public static T InvokeAs<T1, T2, T3, T>(Type type, Func<T1, T2, T3, T> fx, T1 p1, T2 p2, T3 p3)
        {
            return fx.Method.CreateTypedDelegate<Func<T1, T2, T3, T>>(type)(p1, p2, p3);
        }

        public static T InvokeAs<T>(Type type, Type type2, Func<T> fx)
        {
            return fx.Method.CreateTypedDelegate<Func<T>>(type, type2)();
        }

        public static T InvokeAs<T1, T>(Type type, Type type2, Func<T1, T> fx, T1 p1)
        {
            return fx.Method.CreateTypedDelegate<Func<T1, T>>(type, type2)(p1);
        }

        public static T InvokeAs<T1, T2, T>(Type type, Type type2, Func<T1, T2, T> fx, T1 p1, T2 p2)
        {
            return fx.Method.CreateTypedDelegate<Func<T1, T2, T>>(type, type2)(p1, p2);
        }
        public static T InvokeAs<T1, T2, T3, T>(Type type, Type type2, Func<T1, T2, T3, T> fx, T1 p1, T2 p2, T3 p3)
        {
            return fx.Method.CreateTypedDelegate<Func<T1, T2, T3, T>>(type, type2)(p1, p2, p3);
        }

        public static T InvokeAs<Target, T>(this Target target, Type type, Func<T> fx)
        {
            return fx.Method.CreateTypedDelegate<Func<Target, T>>(type)(target);
        }

        public static T InvokeAs<Target, T1, T>(this Target target, Type type, Func<T1, T> fx, T1 p1)
        {
            return fx.Method.CreateTypedDelegate<Func<Target, T1, T>>(type)(target, p1);
        }

        public static T InvokeAs<Target, T1, T2, T>(this Target target, Type type, Func<T1, T2, T> fx, T1 p1, T2 p2)
        {
            return fx.Method.CreateTypedDelegate<Func<Target, T1, T2, T>>(type)(target, p1, p2);
        }
        public static T InvokeAs<Target, T1, T2, T3, T>(this Target target, Type type, Func<T1, T2, T3, T> fx, T1 p1, T2 p2, T3 p3)
        {
            return fx.Method.CreateTypedDelegate<Func<Target, T1, T2, T3, T>>(type)(target, p1, p2, p3);
        }

        public static T InvokeAs<Target, T>(this Target target, Type type, Type type2, Func<T> fx)
        {
            return fx.Method.CreateTypedDelegate<Func<Target,T>>(type,type2)(target);
        }

        public static T InvokeAs<Target, T1, T>(this Target target, Type type, Type type2, Func<T1, T> fx, T1 p1)
        {
            return fx.Method.CreateTypedDelegate<Func<Target, T1, T>>(type, type2)(target, p1);
        }

        public static T InvokeAs<Target, T1, T2, T>(this Target target, Type type, Type type2, Func<T1, T2, T> fx, T1 p1, T2 p2)
        {
            return fx.Method.CreateTypedDelegate<Func<Target, T1, T2, T>>(type, type2)(target, p1, p2);
        }
        public static T InvokeAs<Target, T1, T2, T3, T>(this Target target, Type type, Type type2, Func<T1, T2, T3, T> fx, T1 p1, T2 p2, T3 p3)
        {
            return fx.Method.CreateTypedDelegate<Func<Target, T1, T2, T3, T>>(type, type2)(target, p1, p2, p3);
        }
    }
}
