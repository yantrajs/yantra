using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public static class JSClassBuilder
    {
        static Type type = typeof(JSClass);

        private static ConstructorInfo _New =
            type.Constructor(new Type[] {
                typeof(JSFunctionDelegate), typeof(JSFunction), typeof(string), typeof(string)  });

        private static MethodInfo _AddPrototypeProperty =
            type.InternalMethod(nameof(JSClass.AddPrototypeProperty), typeof(KeyString), typeof(JSFunction), typeof(JSFunction));

        private static MethodInfo _AddPrototypeMethod =
                    type.InternalMethod(nameof(JSClass.AddPrototypeMethod), typeof(KeyString), typeof(JSValue));

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

        public static Expression New(
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
