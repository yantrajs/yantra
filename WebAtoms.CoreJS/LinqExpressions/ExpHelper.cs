using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.LinqExpressions
{
    public static class TypeHelper<T>
    {

        public static Expression StaticProperty(string name)
        {
            return Expression.Property(Expression.Constant(null), typeof(T).GetProperty(name));
        }

        public static Expression New<T1>(Expression p1)
        {
            return Expression.New(Constructor<T1>(), p1);
        }

        public static Expression New<T1>(T1 p1)
            => New<T>(Expression.Constant(p1));

        public static Expression New<T1,T2>(Expression p1, Expression p2)
        {
            return Expression.New(Constructor<T1, T2>(), p1, p2);
        }
        public static Expression New<T1, T2, T3>(Expression p1, Expression p2, Expression p3)
        {
            return Expression.New(Constructor<T1, T2, T3>(), p1, p2, p3);
        }

        public static Expression New<T1, T2>(T1 p1, T2 p2)
            => New<T1, T2>(Expression.Constant(p1), Expression.Constant(p2));
        public static Expression New<T1, T2, T3>(T1 p1, T2 p2, T3 p3)
            => New<T1, T2, T3>(Expression.Constant(p1), Expression.Constant(p2), Expression.Constant(p3));


        public static ConstructorInfo Constructor<T1>()
        {
            return typeof(T).GetConstructor(new Type[] { typeof(T1) });
        }

        public static ConstructorInfo Constructor<T1, T2>()
        {
            return typeof(T).GetConstructor(new Type[] { typeof(T1), typeof(T2) });
        }
        public static ConstructorInfo Constructor<T1, T2, T3>()
        {
            return typeof(T).GetConstructor(new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public static Expression CallStatic<T1>( string name, Expression p1)
        {
            return Expression.Call(Method<T1>(name), p1);
        }
        public static Expression CallStatic<T1, T2>(string name, Expression p1, Expression p2)
        {
            return Expression.Call(Method<T1, T2>(name), p1, p2);
        }

        public static Expression Call<T1>(Expression t, string name, Expression p1)
        {
            return Expression.Call(t, Method<T1>(name), p1);
        }

        public static Expression Call(Expression t, string name, IEnumerable<Expression> p1)
        {
            return Expression.Call(t, Method(name), p1);
        }

        public static Expression CallStatic(string name, IEnumerable<Expression> p1)
        {
            return Expression.Call(Method(name), p1);
        }



        public static Expression Call<T1>(Expression t, string name, T1 p1)
            => Call<T1>(t, name, Expression.Constant(p1));

        public static Expression Call<T1, T2>(Expression t, string name, Expression p1, Expression p2)
        {
            return Expression.Call(t, Method<T1, T2>(name), p1, p2);
        }

        public static Expression Call<T1, T2>(Expression t, string name, T p1, T p2)
            => Call<T1, T2>(t, name, Expression.Constant(p1), Expression.Constant(p2));


        public static MethodInfo Method(string name)
        {
            return typeof(T).GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static MethodInfo Method<T1>(string name)
        {
            return typeof(T).GetMethod(name, new Type[] { typeof(T1) });
        }
        public static MethodInfo Method<T1, T2>(string name)
        {
            return typeof(T).GetMethod(name, new Type[] { typeof(T1), typeof(T2) });
        }

    }

    public static class ExpHelper
    {

        public static Expression JSContextCurrent = Expression.Property( Expression.Constant(null), typeof(JSContext).GetProperty("Current"));

        public static Expression JSContextCurrentScope = Expression.Field(JSContextCurrent, 
            typeof(JSContext).GetField("Scope", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));

        private static PropertyInfo JSContextCurrentScopeIndex =
            typeof(LexicalScope).GetProperties().FirstOrDefault(x => x.GetIndexParameters().Length > 0);

        private static PropertyInfo JSValueDoubleValue =
            typeof(JSValue).GetProperty("DoubleValue");

        private static PropertyInfo JSValueBooleanValue =
            typeof(JSValue).GetProperty("BooleanValue");

        private static PropertyInfo JSValueTypeOf =
            typeof(JSValue).GetProperty("TypeOf");

        private static MethodInfo KeyStringsGetOrCreate =
            typeof(KeyStrings).GetMethod("GetOrCreate");

        private static PropertyInfo JSExceptionError =
            typeof(JSException).GetProperty("Error");

        private static ConstructorInfo NewJSNumber =
            TypeHelper<JSNumber>.Constructor<double>();

        private static MethodInfo JSValueInternalDelete =
            TypeHelper<JSValue>.Method("InternalDelete");

        private static MethodInfo JSValueCreateInstance =
            TypeHelper<JSValue>.Method<JSArray>("CreateInstance");
            
        private static MethodInfo NewJSArray =
            typeof(JSArguments).GetMethod("FromParameters", BindingFlags.NonPublic);

        private static ConstructorInfo NewJSString =
            TypeHelper<JSString>.Constructor<string>();

        public static Expression Undefined =
            Expression.Field(Expression.Constant(null), typeof(JSUndefined).GetField("Value"));

        public static Expression True =
            Expression.Property(JSContextCurrent, "True");

        public static Expression False =
            Expression.Property(JSContextCurrent, "False");


        public static Expression New(Expression callee, IEnumerable<Expression> paramList)
        {
            return Expression.Call(callee, JSValueCreateInstance, Expression.Call(NewJSArray, paramList));
        }

        public static Expression NewArguments(IEnumerable<Expression> args)
        {
            return TypeHelper<JSArguments>.CallStatic("FromParameters", args);
        }

        public static Expression Delete(Expression value, Expression property)
        {
            return Expression.Call(value, JSValueInternalDelete, property);
        }

        public static Expression Throw(Expression value)
        {
            return Expression.Call(Expression.Constant(null),
                typeof(JSException).GetMethod("Throw"),value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyExp"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static Expression AddToScope(Expression keyExp, Expression exp)
        {
            var expParams = new Expression[] { keyExp };
            return Expression.Assign(
                Expression.MakeIndex(JSContextCurrentScope,
                JSContextCurrentScopeIndex,expParams)
                , exp);
        }

        public static Expression KeyOf(string name)
        {
            // KeyStrings.GetOrCreate(name);
            return Expression.Call(Expression.Constant(null), KeyStringsGetOrCreate, Expression.Constant(name));

        }

        //public static Expression KeyOf(Expression name)
        //{
        //    // KeyStrings.GetOrCreate(name);
        //    return Expression.Call(Expression.Constant(null), KeyStringsGetOrCreate, name);

        //}


        public static Expression GetError(ParameterExpression pe)
        {
            return Expression.Property(pe, JSExceptionError);
        }

        public static Expression DoubleValue(Expression exp)
        {
            return Expression.Property(exp, JSValueDoubleValue);
        }

        public static Expression BooleanValue(Expression exp)
        {
            return Expression.Property(exp, JSValueBooleanValue);
        }


        public static Expression JSValueFromDouble(Expression value)
        {
            return Expression.New(NewJSNumber, value);
        }

        public static Expression TypeOf(Expression value)
        {
            return Expression.Property(value, JSValueTypeOf);
        }
    }
}
