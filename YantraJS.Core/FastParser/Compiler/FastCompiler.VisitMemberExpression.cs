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


        protected override Expression VisitMemberExpression(AstMemberExpression memberExpression)
        {
            var isSuper = memberExpression.Object?.Type == FastNodeType.Super;
            var target = isSuper
                ? this.scope.Top.ThisExpression
                : VisitExpression(memberExpression.Object);
            var super = isSuper ? this.scope.Top.Super : null;
            var mp = memberExpression.Property;
            switch (mp.Type)
            {
                case FastNodeType.Identifier: 
                    var  id = mp as AstIdentifier;
                    if (!memberExpression.Computed)
                    {
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(id.Name), memberExpression.Coalesce);
                    }
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        super,
                        VisitIdentifier(id), memberExpression.Coalesce);
                case FastNodeType.Literal:
                    var l = mp as AstLiteral;
                    switch (l.TokenType)
                    {
                        case TokenTypes.True:
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(l.StringValue), memberExpression.Coalesce);
                        case TokenTypes.False:
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(l.StringValue), memberExpression.Coalesce);
                        case TokenTypes.String:
                            var text = l.StringValue;
                            if(Utils.NumberParser.TryCoerceToUInt32(text, out var d))
                            {
                                return ExpHelper.JSValueBuilder.Index(
                                    target,
                                    super,
                                    d, memberExpression.Coalesce);
                            }
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(text), memberExpression.Coalesce);
                        case TokenTypes.Number:
                            var number = l.NumericValue;
                            if (number >= 0 && number < uint.MaxValue && (number % 1) == 0)
                            {
                                return ExpHelper.JSValueBuilder.Index(
                                    target,
                                    super,
                                    (uint)l.NumericValue, memberExpression.Coalesce);
                            }
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(l.NumericValue.ToString()), memberExpression.Coalesce);
                        default:
                            throw new NotImplementedException();
                    }
                case FastNodeType.MemberExpression:
                    var se = mp as AstMemberExpression;
                    return JSValueBuilder.Index(target, super, VisitExpression(se), memberExpression.Coalesce);

            }
            if (memberExpression.Computed)
            {
                return JSValueBuilder.Index(target, super, VisitExpression(memberExpression.Property), memberExpression.Coalesce);
            }
            throw new NotImplementedException();
        }
    }
}
