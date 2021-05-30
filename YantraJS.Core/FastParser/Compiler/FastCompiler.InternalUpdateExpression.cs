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
        private Exp InternalVisitUpdateExpression(AstUnaryExpression updateExpression)
        {
            // added support for a++, a--
            var right = VisitExpression(updateExpression.Argument);
            var ve = Exp.Variable(typeof(JSValue));
            if (updateExpression.Prefix)
            {
                if (updateExpression.Operator == UnaryOperator.Increment)
                {
                    return Exp.Block(new ParameterExpression[] { ve },
                        JSValueExtensionsBuilder.Assign(right, ExpHelper.JSNumberBuilder.New(Exp.Add(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                        JSValueExtensionsBuilder.Assign(ve, right));
                }
                return Exp.Block(new ParameterExpression[] { ve },
                    JSValueExtensionsBuilder.Assign(right, ExpHelper.JSNumberBuilder.New(Exp.Subtract(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                    JSValueExtensionsBuilder.Assign(ve, right));
            }
            if (updateExpression.Operator == UnaryOperator.Increment)
            {
                return Exp.Block(new ParameterExpression[] { ve },
                    JSValueExtensionsBuilder.Assign(ve, right),
                    JSValueExtensionsBuilder.Assign(right, ExpHelper.JSNumberBuilder.New(Exp.Add(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                    ve);
            }
            return Exp.Block(new ParameterExpression[] { ve },
                JSValueExtensionsBuilder.Assign(ve, right),
                JSValueExtensionsBuilder.Assign(right, ExpHelper.JSNumberBuilder.New(Exp.Subtract(DoubleValue(updateExpression.Argument), Exp.Constant((double)1)))),
                ve);
        }
    }
}
