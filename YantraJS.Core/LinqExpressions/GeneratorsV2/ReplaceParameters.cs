using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    internal class ReplaceParameters: YExpressionMapVisitor
    {
        private readonly Dictionary<YExpression, YExpression> replacers;

        public ReplaceParameters(Dictionary<YExpression, YExpression> replacers)
        {
            this.replacers = replacers;
        }

        public override YExpression VisitIn(YExpression exp)
        {
            if (exp == null)
            {
                return null;
            }
            if(replacers.TryGetValue(exp,out var replaced))
            {
                exp = replaced;
            }
            return base.VisitIn(exp);
        }


    }
}
