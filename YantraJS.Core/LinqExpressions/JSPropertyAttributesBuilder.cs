using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Core.LinqExpressions
{
    public static class JSPropertyAttributesBuilder
    {

        public static YExpression Configurable = YExpression.Constant(JSPropertyAttributes.Configurable);

        public static YExpression ConfigurableProperty = YExpression.Constant(JSPropertyAttributes.ConfigurableProperty);

        public static YExpression ConfigurableReadonlyProperty = YExpression.Constant(JSPropertyAttributes.ConfigurableReadonlyProperty);

        public static YExpression ConfigurableReadonlyValue = YExpression.Constant(JSPropertyAttributes.ConfigurableReadonlyValue);

        public static YExpression ConfigurableValue = YExpression.Constant(JSPropertyAttributes.ConfigurableValue);

        public static YExpression Enumerable = YExpression.Constant(JSPropertyAttributes.Enumerable);

        public static YExpression EnumerableConfigurableProperty = YExpression.Constant(JSPropertyAttributes.EnumerableConfigurableProperty);

        public static YExpression EnumerableConfigurableReadonlyProperty = YExpression.Constant(JSPropertyAttributes.EnumerableConfigurableReadonlyProperty);

        public static YExpression EnumerableConfigurableReadonlyValue = YExpression.Constant(JSPropertyAttributes.EnumerableConfigurableReadonlyValue);

        public static YExpression EnumerableConfigurableValue = YExpression.Constant(JSPropertyAttributes.EnumerableConfigurableValue);

        public static YExpression EnumerableReadonlyProperty = YExpression.Constant(JSPropertyAttributes.EnumerableReadonlyProperty);

        public static YExpression EnumerableReadonlyValue = YExpression.Constant(JSPropertyAttributes.EnumerableReadonlyValue);

        public static YExpression Property= YExpression.Constant(JSPropertyAttributes.Property);

        public static YExpression Readonly = YExpression.Constant(JSPropertyAttributes.Readonly);

        public static YExpression ReadonlyProperty = YExpression.Constant(JSPropertyAttributes.ReadonlyProperty);

        public static YExpression ReadonlyValue = YExpression.Constant(JSPropertyAttributes.ReadonlyValue);

        public static YExpression Value = YExpression.Constant(JSPropertyAttributes.Value);

    }
}
