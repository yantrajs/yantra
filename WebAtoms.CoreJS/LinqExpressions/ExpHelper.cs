using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.LinqExpressions
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

        protected static MethodInfo Method(string name)
        {
            return typeof(T).GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic);
        }

        protected static MethodInfo Method<T1>(string name)
        {
            var a = typeof(T).GetMethod(name, new Type[] { typeof(T1) });
            return a;
        }
        protected static MethodInfo Method<T1, T2>(string name)
        {
            return typeof(T).GetMethod(name, new Type[] { typeof(T1), typeof(T2) });
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

    public static class ExpHelper
    {

        public class Exception: TypeHelper<Exception>
        {
            private static MethodInfo _ToString =
                Method("ToString");
            public static Expression ToString(Expression target)
            {
                return Expression.Call(target, _ToString);
            }
        }

        internal class KeyStrings
        {
            private static MethodInfo _GetOrAdd =
                typeof(Core.KeyStrings).GetMethod("GetOrCreate");

            public static Expression GetOrCreate(Expression text)
            {
                return Expression.Call(null, _GetOrAdd, text);
            }
        }

        public class JSContext: TypeHelper<Core.JSContext> {
            public static Expression Current =>
                Expression.Property(null, Property("Current"));

            public static Expression CurrentScope =>
                Expression.Field(Current, InternalField("Scope"));

            public static Expression True =
                Expression.Property(Current, Property("True"));

            public static Expression False =
                Expression.Property(Current, Property("False"));


            private static PropertyInfo _Index =
                IndexProperty<Core.KeyString>();
            public static Expression Index(Expression key)
            {
                return Expression.MakeIndex(Current, _Index, new Expression[] { key });
            }
        }

        public class LexicalScope: TypeHelper<Core.LexicalScope>
        {
            private static PropertyInfo _Index =
                IndexProperty<Core.KeyString>();

            private static FieldInfo _Value =
                typeof(Core.JSVariable).GetField("Value");

            public static Expression Index(Expression exp)
            {
                return Expression.MakeIndex(JSContext.CurrentScope, _Index , new Expression[] { exp });
            }
        }

        public class JSNull: TypeHelper<Core.JSNull>
        {
            public static Expression Value = Expression.Field(null, Field("Value"));
        }

        public class JSUndefined : TypeHelper<Core.JSUndefined>
        {
            public static Expression Value = Expression.Field(null, Field("Value"));
        }


        public class JSNumber: TypeHelper<Core.JSNumber>
        {
            private static ConstructorInfo _NewDouble = Constructor<double>();

            public static Expression New(Expression exp)
            {
                return Expression.New(_NewDouble, exp);
            }

        }

        public class JSString : TypeHelper<Core.JSString>
        {
            private static ConstructorInfo _New = Constructor<string>();

            public static Expression New(Expression exp)
            {
                return Expression.New(_New, exp);
            }

        }
        public class JSRegExp : TypeHelper<Core.JSRegExp>
        {
            private static ConstructorInfo _New = Constructor<string, string>();

            public static Expression New(Expression exp, Expression exp2)
            {
                return Expression.New(_New, exp, exp2);
            }

        }

        public class JSException: TypeHelper<Core.JSException>
        {
            private static MethodInfo _Throw = 
                typeof(Core.JSException).GetMethod("Throw", BindingFlags.NonPublic);

            public static Expression Throw(Expression value)
            {
                return Expression.Call(null, _Throw, value);
            }

            private static PropertyInfo _Error =
                Property("Error");

            public static Expression Error(Expression target)
            {
                return Expression.Property(target, _Error);
            }
        }

        public class JSVariable: TypeHelper<Core.JSVariable>
        {
            private static ConstructorInfo _New
                = Constructor<Core.JSValue, string>();

            public static Expression New(Expression value, string name)
            {
                return Expression.New(_New, value, Expression.Constant(name));
            }

            public static Expression FromArgument(Expression args, uint i, string name)
            {
                return Expression.New(_New, ExpHelper.JSArguments.Index(args,i), Expression.Constant(name));
            }


            public static Expression New(string name)
            {
                return Expression.New(_New, ExpHelper.JSUndefined.Value, Expression.Constant(name));
            }

        }

        public class JSValue: TypeHelper<Core.JSValue>
        {
            private static PropertyInfo _DoubleValue =
                Property("DoubleValue");
            public static Expression DoubleValue(Expression exp)
            {
                return Expression.Property(exp, _DoubleValue);
            }

            private static PropertyInfo _BooleanValue =
                Property("BooleanValue");
            public static Expression BooleanValue(Expression exp)
            {
                return Expression.Property(exp, _BooleanValue);
            }


            private static MethodInfo _CreateInstance =
                Method<Core.JSArray>("CreateInstance");

            public static Expression CreateInstance(Expression target, Expression paramList)
            {
                return Expression.Call(target, _CreateInstance, paramList);
            }

            private static MethodInfo _InvokeMethod =
                Method<Core.KeyString, Core.JSArray>("InvokeMethod");

            public static Expression InvokeMethod(Expression target, Expression keyString, Expression args)
            {
                return Expression.Call(target, _InvokeMethod, keyString, args);
            }

            private static MethodInfo _InvokeFunction =
                Method<Core.KeyString, Core.JSArray>("InvokeFunction");

            public static Expression InvokeFunction(Expression target, Expression t, Expression args)
            {
                return Expression.Call(target, _InvokeFunction, t, args);
            }

            private static MethodInfo _Add =
                Method<Core.JSValue>("Add");

            public static Expression Add(Expression target, Expression value)
            {
                return Expression.Call(target, _Add, value);
            }

            private static MethodInfo _Delete =
                Method<Core.JSValue>("Delete");

            public static Expression Delete(Expression target, Expression value)
            {
                return Expression.Call(target, _Delete, value);
            }

            private static PropertyInfo _TypeOf =
                Property("TypeOf");

            public static Expression TypeOf(Expression target)
            {
                return Expression.Property(target, _TypeOf);
            }

        }

        public class JSArguments: TypeHelper<Core.JSArguments>
        {
            private static ConstructorInfo _New =
                Constructor<Core.JSValue[]>();

            public static Expression New(IEnumerable<Expression> list)
            {
                return Expression.New(_New, list);
            }

            private static PropertyInfo _Index
                = IndexProperty<uint>();

            public static Expression Index(Expression target, uint value)
            {
                return Expression.MakeIndex(target, _Index, new Expression[] {
                    Expression.Constant(value)
                });
            }
        }

        public class JSFunction: TypeHelper<Core.JSFunction>
        {
            private static ConstructorInfo _New =
                Constructor<JSFunctionDelegate, string, string>();

            public static Expression New(Expression del, string name, string code)
            {
                return Expression.New(_New , del, 
                    Expression.Constant(name), 
                    Expression.Constant(code));
            }
        }

    }
}
