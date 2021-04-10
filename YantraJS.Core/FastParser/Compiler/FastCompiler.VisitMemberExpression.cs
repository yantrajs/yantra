using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

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
            switch ((mp.Type, mp))
            {
                case (FastNodeType.Identifier, AstIdentifier id):
                    if (!memberExpression.Computed)
                    {
                        return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(id.Name));
                    }
                    return ExpHelper.JSValueBuilder.Index(
                        target,
                        super,
                        VisitIdentifier(id));
                case (FastNodeType.Literal, AstLiteral l):
                    switch (l.TokenType)
                    {
                        case TokenTypes.True:
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            (uint)1);
                        case TokenTypes.False:
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            (uint)0);
                        case TokenTypes.String:
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(l.StringValue));
                        case TokenTypes.Number:
                            var number = l.NumericValue;
                            if (number >= 0 && number < uint.MaxValue && (number % 1) == 0)
                            {
                                return ExpHelper.JSValueBuilder.Index(
                                    target,
                                    super,
                                    (uint)l.NumericValue);
                            }
                            return ExpHelper.JSValueBuilder.Index(
                            target,
                            super,
                            KeyOfName(l.NumericValue.ToString()));
                        default:
                            throw new NotImplementedException();
                    }
                case (FastNodeType.MemberExpression, AstMemberExpression se):
                    return JSValueBuilder.Index(target, super, VisitExpression(se));

            }
            if (memberExpression.Computed)
            {
                return JSValueBuilder.Index(target, super, VisitExpression(memberExpression.Property));
            }
            throw new NotImplementedException();
        }
    }
}
