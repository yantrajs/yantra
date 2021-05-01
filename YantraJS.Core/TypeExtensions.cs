using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.FastParser;
using YantraJS.ExpHelper;

namespace YantraJS
{
    internal static class ListOfExpressionsExtensions
    {


        internal static Core.FastParser.FastList<Expression> ConvertToInteger(
            this IList<Expression> source, 
            Core.FastParser.FastPool.Scope scope)
        {
            var result = scope.AllocateList<Expression>(source.Count);
            foreach (var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                    throw new NotSupportedException();
                if (ce.Type == typeof(int))
                {
                    result.Add(exp);
                    continue;
                }
                result.Add(Expression.Constant(Convert.ToInt32(ce.Value)));
            }
            return result;
        }

        internal static SparseList<Expression> ConvertToInteger(this IList<Expression> source)
        {
            var result = new SparseList<Expression>(source.Count);
            foreach(var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                    throw new NotSupportedException();
                if (ce.Type == typeof(int))
                {
                    result.Add(exp);
                    continue;
                }
                result.Add(Expression.Constant(Convert.ToInt32(ce.Value)));
            }
            return result;
        }

        internal static FastList<Expression> ConvertToNumber(this FastList<Expression> source, FastPool.Scope scope)
        {
            var result = scope.AllocateList<Expression>(source.Count);
            foreach (var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                    throw new NotSupportedException();
                if (ce.Type == typeof(double))
                {
                    result.Add(exp);
                    continue;
                }
                result.Add(Expression.Constant(Convert.ToDouble(ce.Value)));
            }
            return result;
        }

        internal static SparseList<Expression> ConvertToNumber(this IList<Expression> source)
        {
            var result = new SparseList<Expression>(source.Count);
            foreach (var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                    throw new NotSupportedException();
                if (ce.Type == typeof(double))
                {
                    result.Add(exp);
                    continue;
                }
                result.Add(Expression.Constant(Convert.ToDouble(ce.Value)));
            }
            return result;
        }


        internal static FastList<Expression> ConvertToString(this FastList<Expression> source, FastPool.Scope scope)
        {
            var result = scope.AllocateList<Expression>(source.Count);
            foreach (var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                    throw new NotSupportedException();
                if (ce.Type == typeof(string))
                {
                    result.Add(exp);
                    continue;
                }
                result.Add(Expression.Constant(ce.Value.ToString()));
            }
            return result;
        }

        internal static FastList<Expression> ConvertToJSValue(this FastList<Expression> source, FastPool.Scope scope)
        {
            var result = scope.AllocateList<Expression>(source.Count);
            foreach (var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                {
                    result.Add(exp);
                    continue;
                }
                Expression item;
                switch (ce.Value)
                {
                    case string @string:
                        item = JSStringBuilder.New(ce);
                        break;
                    case double @double:
                        item = JSNumberBuilder.New(ce);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                result.Add(item);
            }
            return result;
        }

        internal static SparseList<Expression> ConvertToString(this IList<Expression> source)
        {
            var result = new SparseList<Expression>(source.Count);
            foreach (var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                    throw new NotSupportedException();
                if (ce.Type == typeof(string))
                {
                    result.Add(exp);
                    continue;
                }
                result.Add(Expression.Constant(ce.Value.ToString()));
            }
            return result;
        }

        internal static SparseList<Expression> ConvertToJSValue(this IList<Expression> source)
        {
            SparseList<Expression> result = new SparseList<Expression>(source.Count);
            foreach (var exp in source)
            {
                if (!(exp is ConstantExpression ce))
                {
                    result.Add(exp);
                    continue;
                }
                Expression item;
                switch (ce.Value) {
                    case string @string:
                        item = JSStringBuilder.New(ce);
                        break;
                    case double @double:
                        item = JSNumberBuilder.New(ce);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                result.Add(item);
            }
            return result;
        }


    }

    internal static class TypeExtensions
    {

        internal static Type GetElementTypeOrGeneric(this Type type)
        {
            if (type.IsArray && type.HasElementType)
            {
                var et = type.GetElementType();
                return et != typeof(object) ? et : null;
            }
            if (type.IsConstructedGenericType)
                return type.GetGenericArguments()[0];
            return null;
        }

        internal static PropertyInfo Property(this Type type , string name)
        {
            var a = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (a == null)
                throw new NullReferenceException($"Property {name} not found on {type.FullName}");
            return a;
        }

        internal static FieldInfo PublicField(this Type type, string name)
        {
            var f = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (f == null)
            {
                throw new NullReferenceException($"Field {name} not found on {type.FullName}");
            }
            return f;
        }


        internal static FieldInfo InternalField(this Type type, string name)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (f == null)
            {
                throw new NullReferenceException($"Field {name} not found on {type.FullName}");
            }
            return f;
        }

        internal static PropertyInfo IndexProperty(this Type type, params Type[] types)
        {
            var px = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .FirstOrDefault(x => x.GetIndexParameters().Length > 0 &&
                x.GetIndexParameters().Select(p => p.ParameterType).SequenceEqual(types));
            if(px == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Property this({tl}) not found on {type.FullName}");
            }
            return px;
        }

        public static MethodInfo PublicMethod(this Type type, string name, params Type[] types)
        {
            var m = type.GetMethod(name,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static MethodInfo InternalMethod(this Type type, string name, params Type[] types)
        {
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }


        public static MethodInfo StaticMethod(this Type type, string name, params Type[] types)
        {
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static MethodInfo StaticMethod<T1>(this Type type, string name)
        {
            var types = new Type[] { typeof(T1) };
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static MethodInfo StaticMethod<T1,T2>(this Type type, string name)
        {
            var types = new Type[] { typeof(T1), typeof(T2) };
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static MethodInfo StaticMethod<T1, T2, T3>(this Type type, string name)
        {
            var types = new Type[] { typeof(T1), typeof(T2), typeof(T3) };
            var m = type.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public,
                null, types, null);
            if (m == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Method {name}({tl}) not found on {type.FullName}");
            }
            return m;
        }

        public static ConstructorInfo PublicConstructor(this Type type, params Type[] types)
        {
            var c = type.GetConstructor(
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance |
                BindingFlags.Public, null,
                types, null);
            if (c == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Constructor {type.Name}({tl}) not found");
            }
            return c;
        }


        public static ConstructorInfo Constructor(this Type type, params Type[] types)
        {
            var c = type.GetConstructor(
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public, null,
                types, null);
            if (c == null)
            {
                var tl = string.Join(",", types.Select(x => x.Name));
                throw new MethodAccessException($"Constructor {type.Name}({tl}) not found");
            }
            return c;
        }

        public static MethodInfo MethodStartsWith(this Type type, string name, params Type[] args)
        {
            var ms = type.GetMethods()
                .Where(x => x.Name == name);
            foreach(var m in ms)
            {
                var pl = m.GetParameters();
                if (pl.Length <= args.Length)
                    continue;
                int i = 0;
                bool found = true;
                foreach(var t in args)
                {
                    if (pl[i++].ParameterType != t)
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return m;
            }
            var tl = string.Join(",", args.Select(x => x.Name));
            throw new MethodAccessException($"Method not found {name}");
        }
    }
}
