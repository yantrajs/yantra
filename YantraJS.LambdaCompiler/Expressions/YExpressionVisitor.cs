﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public abstract class YExpressionVisitor<T>
    {

        public T Visit(YExpression exp)
        {
            switch (exp.NodeType)
            {
                case YExpressionType.Binary:
                    return VisitBinary(exp as YBinaryExpression);
                case YExpressionType.Constant:
                    return VisitConstant(exp as YConstantExpression);
                case YExpressionType.Conditional:
                    return VisitConditional(exp as YConditionalExpression);
                case YExpressionType.Assign:
                    return VistiAssign(exp as YAssignExpression);
                case YExpressionType.Parameter:
                    return VisitParameter(exp as YParameterExpression);
                case YExpressionType.Block:
                    return VisitBlock(exp as YBlockExpression);
                case YExpressionType.Call:
                    return VisitCall(exp as YCallExpression);
                case YExpressionType.New:
                    return VisitNew(exp as YNewExpression);
                case YExpressionType.Field:
                    return VisitField(exp as YFieldExpression);
                case YExpressionType.Property:
                    return VisitProperty(exp as YPropertyExpression);
                case YExpressionType.NewArray:
                    return VisitNewArray(exp as YNewArrayExpression);
                case YExpressionType.GoTo:
                    return VisitGoto(exp as YGoToExpression);
                case YExpressionType.Return:
                    return VisitReturn(exp as YReturnExpression);
                case YExpressionType.Loop:
                    return VisitLoop(exp as YLoopExpression);
                case YExpressionType.Lambda:
                    return VisitLambda(exp as YLambdaExpression);
                case YExpressionType.Label:
                    return VisitLabel(exp as YLabelExpression);
                case YExpressionType.TypeAs:
                    return VisitTypeAs(exp as YTypeAsExpression);
                case YExpressionType.TypeIs:
                    return VisitTypeIs(exp as YTypeIsExpression);
                case YExpressionType.NewArrayBounds:
                    return VisitNewArrayBounds(exp as YNewArrayBoundsExpression);
                case YExpressionType.ArrayIndex:
                    return VisitArrayIndex(exp as YArrayIndexExpression);
                case YExpressionType.Index:
                    return VisitIndex(exp as YIndexExpression);
                default:
                    throw new NotImplementedException($"{exp.NodeType}");
            }
        }

        protected abstract T VisitIndex(YIndexExpression yIndexExpression);
        protected abstract T VisitArrayIndex(YArrayIndexExpression yArrayIndexExpression);
        protected abstract T VisitNewArrayBounds(YNewArrayBoundsExpression yNewArrayBoundsExpression);
        protected abstract T VisitTypeIs(YTypeIsExpression yTypeIsExpression);
        protected abstract T VisitTypeAs(YTypeAsExpression yTypeAsExpression);
        protected abstract T VisitLabel(YLabelExpression yLabelExpression);
        protected abstract T VisitLambda(YLambdaExpression yLambdaExpression);
        protected abstract T VisitLoop(YLoopExpression yLoopExpression);
        protected abstract T VisitReturn(YReturnExpression yReturnExpression);
        protected abstract T VisitGoto(YGoToExpression yGoToExpression);
        protected abstract T VisitNewArray(YNewArrayExpression yNewArrayExpression);
        protected abstract T VisitProperty(YPropertyExpression yPropertyExpression);
        protected abstract T VisitField(YFieldExpression yFieldExpression);
        protected abstract T VisitNew(YNewExpression yNewExpression);
        protected abstract T VisitCall(YCallExpression yCallExpression);
        protected abstract T VisitBlock(YBlockExpression yBlockExpression);
        protected abstract T VisitParameter(YParameterExpression yParameterExpression);
        protected abstract T VistiAssign(YAssignExpression yAssignExpression);
        protected abstract T VisitConditional(YConditionalExpression yConditionalExpression);
        protected abstract T VisitConstant(YConstantExpression yConstantExpression);
        protected abstract T VisitBinary(YBinaryExpression yBinaryExpression);
    }
}