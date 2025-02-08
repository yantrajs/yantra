using System;
using System.Reflection;
using YantraJS.Core.LambdaGen;
using YantraJS.ExpHelper;
using Expression = YantraJS.Expressions.YExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public class GeneratorStateBuilder
    {
        public static Expression New(Expression value, int id, bool @delegate = false)
        {
            return NewLambdaExpression.NewExpression<GeneratorState>(() => () => new GeneratorState(null, 0, false),
                value,
                Expression.Constant(id),
                Expression.Constant(@delegate));
        }

        public static Expression New(int id)
        {
            return NewLambdaExpression.NewExpression<GeneratorState>(() => () => new GeneratorState(null, 0, false),
                JSUndefinedBuilder.Value,
                Expression.Constant(id),
                Expression.Constant(false));
        }

    }
}
