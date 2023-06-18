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

        private Exp DoubleValue(Exp exp)
        {
            return ExpHelper.JSValueBuilder.DoubleValue(exp);
        }

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
                        if (l.TokenType == TokenTypes.BigInt)
                            return JSBigIntBuilder.New("-" + l.StringValue);
                        if (l.TokenType == TokenTypes.String)
                            return ExpHelper.JSNumberBuilder.New(Exp.Negate(DoubleValue(target)));
                    }
                    // return ExpHelper.JSNumberBuilder.New(Exp.Negate(DoubleValue(target)));
                    return JSValueBuilder.Negate(Visit(target));
                case UnaryOperator.BitwiseNot:
                    return ExpHelper.JSNumberBuilder.New(Exp.OnesComplement(JSValueBuilder.IntValue(Visit(target))));
                case UnaryOperator.Negate:
                    return Exp.Condition(BooleanValue(target), JSBooleanBuilder.False, JSBooleanBuilder.True);
                case UnaryOperator.delete:
                    // delete expression...
                    switch (target.Type) {
                        case FastNodeType.Literal:
                            return JSBooleanBuilder.True;
                        case FastNodeType.Identifier:
                            var id = target as AstIdentifier;
                            if(id.Name == "this")
                                return JSBooleanBuilder.True;
                            //return Expression.Throw(
                            //    Expression.New(
                            //        typeof(Exception).PublicConstructor(typeof(string))
                            //        ,Expression.Constant( "Cannot delete a variable")),
                            //    typeof(JSValue)
                            //    );
                            return JSExceptionBuilder.ThrowSyntaxError("Cannot delete a variable in Strict Mode");
                        case FastNodeType.MemberExpression:
                            break;
                        default:
                            return Exp.Block(Visit(target), JSBooleanBuilder.True);
                    }
                    var me = target as AstMemberExpression;
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
                                Exp.Null, Visit(target)), 
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
