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
        private Exp CreateMemberExpression(Exp target, AstExpression property, bool computed)
        {
            switch (property.Type)
            {
                case FastNodeType.Identifier:
                    var id = property as AstIdentifier;
                    if (!computed)
                    {
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            KeyOfName(id.Name));
                    }
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        VisitIdentifier(id));
                case FastNodeType.Literal:
                    var l = property as AstLiteral;
                    switch (l.TokenType)
                    {
                        case TokenTypes.True:
                            return ExpHelper.JSValueBuilder.Index(
                                target,
                                (uint)1);
                        case TokenTypes.False:
                            return ExpHelper.JSValueBuilder.Index(
                                target,
                                (uint)0);
                        case TokenTypes.String:
                            return ExpHelper.JSValueBuilder.Index(
                                target,
                                    KeyOfName(l.Start.CookedText));
                        case TokenTypes.Number:
                            if (l.NumericValue >= 0 && (l.NumericValue % 1 == 0))
                                return ExpHelper.JSValueBuilder.Index(
                                    target,
                                    (uint)l.NumericValue);
                            break;
                    }
                    break;
                case FastNodeType.MemberExpression:
                    var se = property as AstMemberExpression;
                    return JSValueBuilder.Index(target, Visit(se.Property));

            }
            if (computed)
            {
                return JSValueBuilder.Index(target, Visit(property));
            }

            throw new NotImplementedException();
        }
    }
}
