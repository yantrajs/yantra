using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;


namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Exp VisitBreakStatement(AstBreakStatement breakStatement)
        {
            var ls = this.LoopScope;

            string name = breakStatement.Label?.Name.Value;
            if (name != null)
            {
                var target = this.LoopScope.Get(name);
                if (target == null)
                    throw JSContext.Current.NewSyntaxError($"No label found for {name}");
                return Exp.Break(target.Break);
            }

            if (ls.IsSwitch)
                return Exp.Goto(ls.Break);

            return Exp.Break(ls.Break);
        }
    }
}
