using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;
using YantraJS.Core.CodeGen;

namespace YantraJS.ExpHelper
{
    public static class JSClassBuilder
    {
        static Type type = typeof(JSClass);

        private static ConstructorInfo _New =
            type.Constructor(new Type[] {
                typeof(ScriptInfo),
                typeof(JSVariable[]),
                typeof(JSClosureFunctionDelegate), typeof(JSFunction), typeof(string), typeof(string)  });

        private static MethodInfo _AddPrototypeProperty =
            type.PublicMethod(nameof(JSClass.AddPrototypeProperty), KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction));

        private static MethodInfo _AddPrototypeMethod =
                    type.PublicMethod(nameof(JSClass.AddPrototypeMethod), KeyStringsBuilder.RefType, typeof(JSValue));
        private static MethodInfo _AddPrototypeValueMethod =
                    type.PublicMethod(nameof(JSClass.AddPrototypeMethod), typeof(JSValue), typeof(JSValue));

        private static MethodInfo _AddStaticProperty =
            type.PublicMethod(nameof(JSClass.AddStaticProperty), KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction));

        private static MethodInfo _AddStaticMethod =
                    type.PublicMethod(nameof(JSClass.AddStaticMethod), KeyStringsBuilder.RefType, typeof(JSValue));

        public static Expression AddValue(
            Expression target,
            Expression name,
            Expression value)
        {
            if (name.Type == typeof(JSValue))
                return Expression.Call(
                    target,
                    _AddPrototypeValueMethod,
                    name,
                    value);
            return Expression.Call(
                target,
                _AddPrototypeMethod,
                name,
                value);
        }


        public static Expression AddProperty(
            Expression target,
            Expression name,
            Expression getter,
            Expression setter)
        {
            return Expression.Call(
                target,
                _AddPrototypeProperty,
                name,
                getter ?? Expression.Constant(null, typeof(JSFunction)),
                setter ?? Expression.Constant(null, typeof(JSFunction)));
        }

        public static Expression AddStaticValue(
            Expression target,
            Expression name,
            Expression value)
        {
            return Expression.Call(
                target,
                _AddStaticMethod,
                name,
                value);
        }


        public static Expression AddStaticProperty(
            Expression target,
            Expression name,
            Expression getter,
            Expression setter)
        {
            return Expression.Call(
                target,
                _AddStaticProperty,
                name,
                getter ?? Expression.Constant(null, typeof(JSFunction)),
                setter ?? Expression.Constant(null, typeof(JSFunction)));
        }


        public static Expression New(
            Expression scriptInfo,
            Expression closures,
            Expression constructor,
            Expression super,
            string name,
            string code = "")
        {
            return Expression.New(_New,
                scriptInfo,
                closures ?? Expression.Constant(null,typeof(JSVariable[])),
                constructor ?? Expression.Constant(null, typeof(JSClosureFunctionDelegate)),
                super ?? Expression.Constant(null, typeof(JSFunction)),
                Expression.Constant(name),
                Expression.Constant(code));
        }
    }
}
