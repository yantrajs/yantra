using System;
using System.Linq;
using System.Reflection;

namespace YantraJS.ExpHelper
{
    public class TypeHelper<T>
    {

        protected static PropertyInfo IndexProperty<T1>()
        {
            return typeof(T)
                .GetProperties()
                .FirstOrDefault(x => x.GetIndexParameters().Length > 0 &&
                x.GetIndexParameters()[0].ParameterType == typeof(T1));
        }

        protected static PropertyInfo Property(string name)
        {
            var a = typeof(T).GetProperty(name);
            if (a == null)
                throw new NullReferenceException($"Property {name} not found on {typeof(T).FullName}");
            return a;
        }

        protected static ConstructorInfo Constructor<T1>()
        {
            var a = typeof(T).GetConstructor(new Type[] { typeof(T1) });
            return a;
        }

        protected static ConstructorInfo Constructor<T1, T2>()
        {
            return typeof(T).GetConstructor(new Type[] { typeof(T1), typeof(T2) });
        }
        protected static ConstructorInfo Constructor<T1, T2, T3>()
        {
            var c = typeof(T).GetConstructor(new Type[] { typeof(T1), typeof(T2), typeof(T3) });
            if (c != null)
                return c;
            var list = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);
            return list.FirstOrDefault(x =>
                        x.GetParameters().Length == 3
                        && x.GetParameters()[0].ParameterType == typeof(T1)
                        && x.GetParameters()[1].ParameterType == typeof(T2)
                        && x.GetParameters()[2].ParameterType == typeof(T3));
        }

        protected static ConstructorInfo Constructor<T1, T2, T3, T4>()
        {
            var c = typeof(T).GetConstructor(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            if (c != null)
                return c;
            var list = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);
            return list.FirstOrDefault(x =>
                        x.GetParameters().Length == 4
                        && x.GetParameters()[0].ParameterType == typeof(T1)
                        && x.GetParameters()[1].ParameterType == typeof(T2)
                        && x.GetParameters()[2].ParameterType == typeof(T3)
                        && x.GetParameters()[3].ParameterType == typeof(T4));
        }


        protected static MethodInfo Method(string name)
        {
            return typeof(T).GetMethod(name);
        }

        protected static MethodInfo Method<T1>(string name)
        {
            var a = typeof(T).GetMethod(name, new Type[] { typeof(T1) });
            return a;
        }

        protected static MethodInfo InternalMethod(string name)
        {
            var a = typeof(T)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Static);
            return a.First(x => x.Name == name);
        }

        protected static MethodInfo InternalMethod<T1>(string name)
        {
            var a = typeof(T)
                .GetMethod(name,
                    BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Static
                    , null, new Type[] { typeof(T1) }, null);
            return a;
        }

        protected static MethodInfo InternalMethod<T1, T2>(string name)
        {
            var a = typeof(T)
                .GetMethod(name,
                    BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public
                    , null, new Type[] { typeof(T1), typeof(T2) }, null);
            return a;
        }

        protected static MethodInfo Method<T1, T2>(string name)
        {
            return typeof(T).GetMethod(name, new Type[] { typeof(T1), typeof(T2) });
        }

        protected static MethodInfo StaticMethod<T1, T2>(string name)
        {
            return typeof(T)
                .GetMethod(name,
                new Type[] { typeof(T1), typeof(T2) });
        }

        protected static MethodInfo InternalStaticMethod<T1>(string name)
        {
            var a = typeof(T)
                .GetMethod(name,
                    BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Static
                    , null, new Type[] { typeof(T1) }, null);
            return a;
        }

        protected static MethodInfo InternalStaticMethod<T1, T2>(string name)
        {
            var a = typeof(T)
                .GetMethod(name,
                    BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Static
                    , null, new Type[] { typeof(T1), typeof(T2) }, null);
            return a;
        }

        protected static FieldInfo InternalField(string name)
        {
            return typeof(T).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected static FieldInfo Field(string name)
        {
            return typeof(T).GetField(name);
        }

    }
}
