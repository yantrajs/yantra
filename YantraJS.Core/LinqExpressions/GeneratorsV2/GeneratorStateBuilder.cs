using System;
using System.Reflection;
using YantraJS.ExpHelper;
using Expression = YantraJS.Expressions.YExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public class GeneratorStateBuilder
    {
        private static Type type = typeof(GeneratorState);

        private static ConstructorInfo _newFromValue =
            type.PublicConstructor(typeof(JSValue), typeof(int));

        public static Expression New(Expression value, int id)
        {
            return Expression.New(_newFromValue, value, Expression.Constant(id));
        }

        public static Expression New(int id)
        {
            return Expression.New(_newFromValue, JSUndefinedBuilder.Value, Expression.Constant(id));
        }

    }
}
