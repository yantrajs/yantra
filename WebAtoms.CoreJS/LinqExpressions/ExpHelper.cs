using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.LinqExpressions
{
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
            typeof(JSNumber).GetConstructor(new Type[] { typeof(double) });

        public static Expression Undefined =
            Expression.Field(Expression.Constant(null), typeof(JSUndefined).GetField("Value"));

        

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
