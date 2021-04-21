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
        private Exp DoubleValue(AstExpression exp)
        {
            return ExpHelper.JSValueBuilder.DoubleValue(VisitExpression(exp));
        }

        private Exp BooleanValue(AstExpression exp)
        {
            return ExpHelper.JSValueBuilder.BooleanValue(VisitExpression(exp));
        }

        protected override Expression VisitUnaryExpression(AstUnaryExpression unaryExpression)
        {
            var target = unaryExpression.Argument;

            switch (unaryExpression.Operator)
            {
                case UnaryOperator.Plus:
                    return ExpHelper.JSNumberBuilder.New(Exp.UnaryPlus(DoubleValue(target)));
                case UnaryOperator.Minus:
                    if (target.Type == FastNodeType.Literal)
                    {
                        AstLiteral l = unaryExpression.Argument as AstLiteral;
                        if (l.TokenType == TokenTypes.Number)
                            return JSNumberBuilder.New(Exp.Constant(-l.NumericValue));
                    }
                    return ExpHelper.JSNumberBuilder.New(Exp.Negate(DoubleValue(target)));
                case UnaryOperator.BitwiseNot:
                    return ExpHelper.JSNumberBuilder.New(Exp.Not(JSValueBuilder.IntValue(Visit(target))));
                case UnaryOperator.Negate:
                    return Exp.Condition(BooleanValue(target), JSBooleanBuilder.False, JSBooleanBuilder.True);
                case UnaryOperator.delete:
                    // delete expression...
                    var me = target as AstMemberExpression;
                    if (me == null)
                        return JSBooleanBuilder.False;
                    var targetObj = VisitExpression(me.Object);
                    if (me.Computed)
                    {
                        Exp pe = VisitExpression(me.Property);
                        return JSValueBuilder.Delete(targetObj, pe);
                    }
                    else
                    {
                        var mep = me.Property;
                        switch (mep.Type)
                        {
                            case FastNodeType.Literal:
                                AstLiteral l = mep as AstLiteral;
                                if (l.TokenType == TokenTypes.Number)
                                    return JSValueBuilder.Delete(targetObj, Exp.Constant((uint)l.NumericValue));
                                if (l.TokenType == TokenTypes.String)
                                    return JSValueBuilder.Delete(targetObj, KeyOfName(l.StringValue));
                                break;
                            case FastNodeType.Identifier:
                                AstIdentifier id = mep as AstIdentifier;
                                return JSValueBuilder.Delete(targetObj, KeyOfName(id.Name));
                        }
                    }
                    break;
                case UnaryOperator.@void:
                    if (target != null && target.Type != FastNodeType.Literal)
                        return Exp.Condition(
                            Exp.Equal(
                                Exp.Constant(null, typeof(JSValue)), Visit(target)), 
                            JSUndefinedBuilder.Value, 
                            JSUndefinedBuilder.Value);
                    return ExpHelper.JSUndefinedBuilder.Value;
                case UnaryOperator.@typeof:
                    return ExpHelper.JSValueBuilder.TypeOf(VisitExpression(target));
                case UnaryOperator.Increment:
                    return this.InternalVisitUpdateExpression(unaryExpression);
                case UnaryOperator.Decrement:
                    return this.InternalVisitUpdateExpression(unaryExpression);
            }
            throw new InvalidOperationException();
        }
    }
}
