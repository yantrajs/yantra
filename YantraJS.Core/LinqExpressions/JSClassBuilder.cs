using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.CodeGen;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Expressions;

namespace YantraJS.ExpHelper
{
    public static class JSClassBuilder
    {
        static Type type = typeof(JSClass);

        private static ConstructorInfo _New =
            type.Constructor(new Type[] {
                typeof(JSFunctionDelegate), typeof(JSFunction), typeof(string), typeof(string)  });

        public static MethodInfo _AddConstructor =
            type.PublicMethod(nameof(JSClass.AddConstructor), typeof(JSFunction));

        public static YElementInit AddConstructor(YExpression exp)
        {
            return YExpression.ElementInit(_AddConstructor, exp);
        }

        //private static MethodInfo _AddPrototypeProperty =
        //    type.PublicMethod(nameof(JSClass.AddPrototypeProperty), KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction));

        //private static MethodInfo _AddPrototypeMethod =
        //            type.PublicMethod(nameof(JSClass.AddPrototypeMethod), KeyStringsBuilder.RefType, typeof(JSValue));
        //private static MethodInfo _AddPrototypeValueMethod =
        //            type.PublicMethod(nameof(JSClass.AddPrototypeMethod), typeof(JSValue), typeof(JSValue));

        //private static MethodInfo _AddStaticProperty =
        //    type.PublicMethod(nameof(JSClass.AddStaticProperty), KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction));

        //private static MethodInfo _AddStaticMethod =
        //            type.PublicMethod(nameof(JSClass.AddStaticMethod), KeyStringsBuilder.RefType, typeof(JSValue));

        //public static Expression AddValue(Expression target, Expression  name, Expression value, bool isStatic)
        //{
        //    return JSObjectBuilder.AddValue(isStatic 
        //        ? target
        //        : JSFunctionBuilder.Prototype(target), name, value, JSPropertyAttributes.ConfigurableValue);
        //}

        //public static Expression AddProperty(Expression target, Expression name, Expression getter, Expression setter, bool isStatic)
        //{
        //    return JSObjectBuilder.AddProperty( isStatic 
        //        ? target 
        //        : JSFunctionBuilder.Prototype(target), name, getter ?? Expression.Null, setter ?? Expression.Null, JSPropertyAttributes.ConfigurableProperty);
        //}

        //public static Expression AddValue(
        //    Expression target,
        //    Expression name,
        //    Expression value)
        //{
        //    if (name.Type == typeof(JSValue))
        //        return Expression.Call(
        //            target,
        //            _AddPrototypeValueMethod,
        //            name,
        //            value);
        //    return Expression.Call(
        //        target,
        //        _AddPrototypeMethod,
        //        name,
        //        value);
        //}


        //public static Expression AddProperty(
        //    Expression target,
        //    Expression name,
        //    Expression getter,
        //    Expression setter)
        //{
        //    return Expression.Call(
        //        target,
        //        _AddPrototypeProperty,
        //        name,
        //        getter ?? Expression.Constant(null, typeof(JSFunction)),
        //        setter ?? Expression.Constant(null, typeof(JSFunction)));
        //}

        //public static Expression AddStaticValue(
        //    Expression target,
        //    Expression name,
        //    Expression value)
        //{
        //    return Expression.Call(
        //        target,
        //        _AddStaticMethod,
        //        name,
        //        value);
        //}


        //public static Expression AddStaticProperty(
        //    Expression target,
        //    Expression name,
        //    Expression getter,
        //    Expression setter)
        //{
        //    return Expression.Call(
        //        target,
        //        _AddStaticProperty,
        //        name,
        //        getter ?? Expression.Constant(null, typeof(JSFunction)),
        //        setter ?? Expression.Constant(null, typeof(JSFunction)));
        //}


        public static YNewExpression New(
            Expression constructor,
            Expression super,
            string name,
            string code = "")
        {
            return Expression.New(_New,
                constructor ?? Expression.Constant(null, typeof(JSFunctionDelegate)),
                super ?? Expression.Constant(null, typeof(JSFunction)),
                Expression.Constant(name),
                Expression.Constant(code));
        }
    }
}
