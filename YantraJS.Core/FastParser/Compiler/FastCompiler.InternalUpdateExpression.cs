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
using YantraJS.Expressions;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        private Exp InternalVisitUpdateExpression(AstUnaryExpression updateExpression)
        {
            // added support for a++, a--
            updateExpression.Argument.VerifyIdentifierForUpdate();


            var list = new Sequence<Exp>();

            FastFunctionScope.VariableScope target = null;
            FastFunctionScope.VariableScope @return = null;
            // try
            // {
                var right = VisitExpression(updateExpression.Argument);

                switch (right.NodeType)
                {
                    case YExpressionType.Index:
                        var index = right as YIndexExpression;
                        target = this.scope.Top.GetTempVariable(index.Type);
                        list.Add(Exp.Assign(target.Variable, index.Target));
                        right = Exp.Index(target.Variable, index.Property, index.Arguments);
                        break;
                }

                if (!updateExpression.Prefix)
                {
                    @return = this.scope.Top.GetTempVariable(right.Type);
                    list.Add(Exp.Assign(@return.Variable, right));
                }

                switch (updateExpression.Operator)
                {
                    case UnaryOperator.Increment:
                        list.Add(Exp.Assign(right, ExpHelper.JSValueBuilder.AddDouble(right, Exp.Constant((double)1))));
                        break;
                    case UnaryOperator.Decrement:
                        list.Add(Exp.Assign(right, ExpHelper.JSValueBuilder.AddDouble(right, Exp.Constant((double)-1))));
                        break;
                }
                if (!updateExpression.Prefix)
                {
                    list.Add(@return.Variable);
                }
                else
                {
                    list.Add(right);
                }

                var r = Exp.Block(list);
                @return?.Dispose();
                target?.Dispose();
                // list.Clear();
                return r;
            //} finally
            //{
            //    @return?.Dispose();
            //    target?.Dispose();
            //    list.Clear();
            //}
        }
    }
}
