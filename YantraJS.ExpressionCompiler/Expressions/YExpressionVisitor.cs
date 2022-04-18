using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public abstract class YExpressionVisitor<T>: StackGuard<T, YExpression>
    {
        public override T VisitIn(YExpression exp)
        {
            if (exp == null)
                return default;
            switch (exp.NodeType)
            {
                case YExpressionType.Block:
                    return VisitBlock(exp as YBlockExpression);
                case YExpressionType.Call:
                    return VisitCall(exp as YCallExpression);
                case YExpressionType.Binary:
                    return VisitBinary(exp as YBinaryExpression);
                case YExpressionType.Constant:
                    return VisitConstant(exp as YConstantExpression);
                case YExpressionType.Conditional:
                    return VisitConditional(exp as YConditionalExpression);
                case YExpressionType.Assign:
                    return VisitAssign(exp as YAssignExpression);
                case YExpressionType.Parameter:
                    return VisitParameter(exp as YParameterExpression);
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
                case YExpressionType.Coalesce:
                    return VisitCoalesce(exp as YCoalesceExpression);
                case YExpressionType.Unary:
                    return VisitUnary(exp as YUnaryExpression);
                case YExpressionType.ArrayLength:
                    return VisitArrayLength(exp as YArrayLengthExpression);
                case YExpressionType.TryCatchFinally:
                    return VisitTryCatchFinally(exp as YTryCatchFinallyExpression);
                case YExpressionType.Throw:
                    return VisitThrow(exp as YThrowExpression);
                case YExpressionType.Convert:
                    return VisitConvert(exp as YConvertExpression);
                case YExpressionType.Invoke:
                    return VisitInvoke(exp as YInvokeExpression);
                case YExpressionType.Delegate:
                    return VisitDelegate(exp as YDelegateExpression);
                case YExpressionType.MemberInit:
                    return VisitMemberInit(exp as YMemberInitExpression);
                //case YExpressionType.Relay:
                //    return VisitRelay(exp as YRelayExpression);
                case YExpressionType.Empty:
                    return VisitEmpty(exp as YEmptyExpression);
                case YExpressionType.Switch:
                    return VisitSwitch(exp as YSwitchExpression);
                case YExpressionType.Yield:
                    return VisitYield(exp as YYieldExpression);
                case YExpressionType.DebugInfo:
                    return VisitDebugInfo(exp as YDebugInfoExpression);
                case YExpressionType.ILOffset:
                    return VisitILOffset(exp as YILOffsetExpression);
                case YExpressionType.Box:
                    return VisitBox(exp as YBoxExpression);
                case YExpressionType.Unbox:
                    return VisitUnbox(exp as YUnboxExpression);
                case YExpressionType.JumpSwitch:
                    return VisitJumpSwitch(exp as YJumpSwitchExpression);
                case YExpressionType.ListInit:
                    return VisitListInit(exp as YListInitExpression);
                case YExpressionType.CoalesceCall:
                    return VisitCoalesceCall(exp as YCoalesceCallExpression);
                //case YExpressionType.TypeEqual:
                //    break;
                case YExpressionType.Int32Constant:
                    return VisitInt32Constant(exp as YInt32ConstantExpression);
                case YExpressionType.UInt32Constant:
                    return VisitUInt32Constant(exp as YUInt32ConstantExpression);
                case YExpressionType.Int64Constant:
                    return VisitInt64Constant(exp as YInt64ConstantExpression);
                case YExpressionType.UInt64Constant:
                    return VisitUInt64Constant(exp as YUInt64ConstantExpression);
                case YExpressionType.DoubleConstant:
                    return VisitDoubleConstant(exp as YDoubleConstantExpression);
                case YExpressionType.FloatConstant:
                    return VisitFloatConstant(exp as YFloatConstantExpression);
                case YExpressionType.BooleanConstant:
                    return VisitBooleanConstant(exp as YBooleanConstantExpression);
                case YExpressionType.StringConstant:
                    return VisitStringConstant(exp as YStringConstantExpression);
                case YExpressionType.ByteConstant:
                    return VisitByteConstant(exp as YByteConstantExpression);
                case YExpressionType.TypeConstant:
                    return VisitTypeConstant(exp as YTypeConstantExpression);
                case YExpressionType.MethodConstant:
                    return VisitMethodConstant(exp as YMethodConstantExpression);
                case YExpressionType.AddressOf:
                    return VisitAddressOf(exp as YAddressOfExpression);
                default:
                    throw new NotImplementedException($"{exp.NodeType}");
            }
        }

        protected abstract T VisitAddressOf(YAddressOfExpression node);
        protected abstract T VisitMethodConstant(YMethodConstantExpression node);
        protected abstract T VisitTypeConstant(YTypeConstantExpression node);
        protected abstract T VisitByteConstant(YByteConstantExpression node);
        protected abstract T VisitStringConstant(YStringConstantExpression node);
        protected abstract T VisitBooleanConstant(YBooleanConstantExpression node);
        protected abstract T VisitFloatConstant(YFloatConstantExpression node);
        protected abstract T VisitDoubleConstant(YDoubleConstantExpression node);
        protected abstract T VisitUInt64Constant(YUInt64ConstantExpression node);
        protected abstract T VisitInt64Constant(YInt64ConstantExpression node);
        protected abstract T VisitUInt32Constant(YUInt32ConstantExpression node);
        protected abstract T VisitInt32Constant(YInt32ConstantExpression node);
        protected abstract T VisitCoalesceCall(YCoalesceCallExpression node);
        protected abstract T VisitListInit(YListInitExpression node);
        protected abstract T VisitJumpSwitch(YJumpSwitchExpression node);
        protected abstract T VisitUnbox(YUnboxExpression node);
        protected abstract T VisitBox(YBoxExpression node);
        protected abstract T VisitILOffset(YILOffsetExpression node);
        protected abstract T VisitDebugInfo(YDebugInfoExpression node);
        protected abstract T VisitYield(YYieldExpression node);
        protected abstract T VisitSwitch(YSwitchExpression node);
        protected abstract T VisitEmpty(YEmptyExpression exp);
        // protected abstract T VisitRelay(YRelayExpression yRelayExpression);
        protected abstract T VisitMemberInit(YMemberInitExpression memberInitExpression);
        protected abstract T VisitDelegate(YDelegateExpression yDelegateExpression);
        protected abstract T VisitInvoke(YInvokeExpression invokeExpression);
        protected abstract T VisitConvert(YConvertExpression convertExpression);
        protected abstract T VisitThrow(YThrowExpression throwExpression);
        protected abstract T VisitTryCatchFinally(YTryCatchFinallyExpression tryCatchFinallyExpression);
        protected abstract T VisitArrayLength(YArrayLengthExpression arrayLengthExpression);
        protected abstract T VisitUnary(YUnaryExpression yUnaryExpression);
        protected abstract T VisitCoalesce(YCoalesceExpression yCoalesceExpression);
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
        protected abstract T VisitAssign(YAssignExpression yAssignExpression);
        protected abstract T VisitConditional(YConditionalExpression yConditionalExpression);
        protected abstract T VisitConstant(YConstantExpression yConstantExpression);
        protected abstract T VisitBinary(YBinaryExpression yBinaryExpression);
    }
}
