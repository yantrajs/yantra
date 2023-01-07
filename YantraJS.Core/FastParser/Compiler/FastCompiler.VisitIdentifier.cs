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
        protected override Expression VisitIdentifier(AstIdentifier identifier)
        {
            if (identifier.Name.Equals("undefined"))
                return JSUndefinedBuilder.Value;
            if (identifier.Name.Equals("this"))
                return this.scope.Top.ThisExpression;
            if (identifier.Name.Equals("arguments"))
            {
                var functionScope = this.scope.Top.RootScope;
                var vs = functionScope.CreateVariable("arguments",
                    JSArgumentsBuilder.New(functionScope.ArgumentsExpression));
                return vs.Expression;

            }
            var var = this.scope.Top.GetVariable(identifier.Name, true);
            if (var != null)
                return var.Expression;

            return ExpHelper.JSContextBuilder.Index(KeyOfName(identifier.Name));
        }
    }
}
