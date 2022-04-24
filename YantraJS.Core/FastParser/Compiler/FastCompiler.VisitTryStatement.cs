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

        protected override Expression VisitTryStatement(AstTryStatement tryStatement)
        {
            var block = VisitStatement(tryStatement.Block);
            var cb = tryStatement.Catch;
            if (cb != null)
            {
                var id = tryStatement.Identifier;
                var pe = this.scope.Top.CreateException(id.Name.Value);
                using var scope = this.scope.Push(new FastFunctionScope(this.scope.Top));
                var v = scope.CreateVariable(id.Name, newScope: true);

                var catchBlock = Exp.Block(v.Variable.AsSequence(),
                    Exp.Assign(v.Variable, ExpHelper.JSVariableBuilder.NewFromException(pe.Variable, id.Name.Value)),
                    VisitStatement(cb));
                var cbExp = Exp.Catch(pe.Variable, catchBlock.ToJSValue());


                if (tryStatement.Finally != null)
                {
                    return Exp.TryCatchFinally(block.ToJSValue(), VisitStatement(tryStatement.Finally).ToJSValue(), cbExp);
                }

                return Exp.TryCatch(block.ToJSValue(), cbExp);
            }

            var @finally = tryStatement.Finally;
            if (@finally != null)
            {
                return Exp.TryFinally(block.ToJSValue(), VisitStatement(@finally).ToJSValue());
            }

            return JSUndefinedBuilder.Value;
        }
    }
}
