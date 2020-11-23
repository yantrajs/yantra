using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public static class JSClassBuilder
    {
        static Type type = typeof(JSClass);

        private static ConstructorInfo _New =
            type.Constructor(new Type[] {
                typeof(JSVariable[]),
                typeof(JSClosureFunctionDelegate), typeof(JSFunction), typeof(string), typeof(string)  });

        private static MethodInfo _AddPrototypeProperty =
            type.InternalMethod(nameof(JSClass.AddPrototypeProperty), typeof(KeyString), typeof(JSFunction), typeof(JSFunction));

        private static MethodInfo _AddPrototypeMethod =
                    type.InternalMethod(nameof(JSClass.AddPrototypeMethod), typeof(KeyString), typeof(JSValue));

        private static MethodInfo _AddStaticProperty =
            type.InternalMethod(nameof(JSClass.AddStaticProperty), typeof(KeyString), typeof(JSFunction), typeof(JSFunction));

        private static MethodInfo _AddStaticMethod =
                    type.InternalMethod(nameof(JSClass.AddStaticMethod), typeof(KeyString), typeof(JSValue));

        public static Expression AddValue(
            Expression target,
            Expression name,
            Expression value)
        {
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
            Expression closures,
            Expression constructor,
            Expression super,
            string name,
            string code = "")
        {
            return Expression.New(_New,
                closures ?? Expression.Constant(null,typeof(JSVariable[])),
                constructor ?? Expression.Constant(null, typeof(JSClosureFunctionDelegate)),
                super ?? Expression.Constant(null, typeof(JSFunction)),
                Expression.Constant(name),
                Expression.Constant(code));
        }
    }
}
