//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using YantraJS.Core;

//namespace YantraJS.ExpHelper
//{
//    public class JSPropertyBuilder
//    {
//        private static Type type = typeof(JSProperty);

//        private static ConstructorInfo _New =
//            type.GetConstructors().FirstOrDefault();

//        private static FieldInfo _Attributes =
//            type.InternalField(nameof(Core.JSProperty.Attributes));

//        private static FieldInfo _Key =
//            type.InternalField(nameof(Core.JSProperty.key));

//        private static FieldInfo _Get =
//            type.InternalField(nameof(Core.JSProperty.get));

//        private static FieldInfo _Value =
//            type.InternalField(nameof(Core.JSProperty.value));

//        public static Expression Value(Expression key, Expression value)
//        {
//            return Expression.MemberInit(Expression.New(typeof(Core.JSProperty)),
//                Expression.Bind(_Key, key),
//                Expression.Bind(_Value, value),
//                Expression.Bind(_Attributes, Expression.Constant(JSPropertyAttributes.EnumerableConfigurableValue))
//                );
//        }

//        public static Expression Property(Expression key, Expression getter, Expression setter)
//        {
//            getter = getter == null
//                ? (Expression)Expression.Constant(null, typeof(Core.JSFunction))
//                : Expression.Convert(getter, typeof(Core.JSFunction));
//            setter = setter == null
//                ? (Expression)Expression.Constant(null, typeof(Core.JSFunction))
//                : Expression.Convert(setter, typeof(Core.JSFunction));
//            return Expression.MemberInit(Expression.New(typeof(Core.JSProperty)),
//                Expression.Bind(_Key, key),
//                Expression.Bind(_Get, getter),
//                Expression.Bind(_Value, setter),
//                Expression.Bind(_Attributes, Expression.Constant(JSPropertyAttributes.EnumerableConfigurableReadonlyProperty))
//                );
//        }

//    }
//}
