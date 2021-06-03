using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS.Converters
{
    public abstract class LinqMap<T>: StackGuard<T, Expression>
        where T: class
    {
        public override T VisitIn(Expression exp)
        {
            if (exp == null)
                return default;
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
					return VisitAdd(exp as BinaryExpression);
                case ExpressionType.AddAssign:
					return VisitAddAssign(exp as BinaryExpression);
                case ExpressionType.AddAssignChecked:
					return VisitAddAssignChecked(exp as BinaryExpression);
                case ExpressionType.AddChecked:
					return VisitAddChecked(exp as BinaryExpression);
                case ExpressionType.And:
					return VisitAnd(exp as BinaryExpression);
                case ExpressionType.AndAlso:
					return VisitAndAlso(exp as BinaryExpression);
                case ExpressionType.AndAssign:
					return VisitAndAssign(exp as BinaryExpression);
                case ExpressionType.ArrayIndex:
					return VisitArrayIndex(exp as BinaryExpression);
                case ExpressionType.ArrayLength:
					return VisitArrayLength(exp as UnaryExpression);
                case ExpressionType.Assign:
					return VisitAssign(exp as BinaryExpression);
                case ExpressionType.Block:
					return VisitBlock(exp as BlockExpression);
                case ExpressionType.Call:
					return VisitCall(exp as MethodCallExpression);
                case ExpressionType.Coalesce:
					return VisitCoalesce(exp as BinaryExpression);
                case ExpressionType.Conditional:
					return VisitConditional(exp as ConditionalExpression);
                case ExpressionType.Constant:
					return VisitConstant(exp as ConstantExpression);
                case ExpressionType.Convert:
					return VisitConvert(exp as UnaryExpression);
                case ExpressionType.ConvertChecked:
					return VisitConvertChecked(exp as UnaryExpression);
                case ExpressionType.DebugInfo:
					return VisitDebugInfo(exp as ConstantExpression);
                case ExpressionType.Decrement:
					return VisitDecrement(exp as UnaryExpression);
                case ExpressionType.Default:
					return VisitDefault(exp as DefaultExpression);
                case ExpressionType.Divide:
					return VisitDivide(exp as BinaryExpression);
                case ExpressionType.DivideAssign:
					return VisitDivideAssign(exp as BinaryExpression);
                case ExpressionType.Dynamic:
					return VisitDynamic(exp as DynamicExpression);
                case ExpressionType.Equal:
					return VisitEqual(exp as BinaryExpression);
                case ExpressionType.ExclusiveOr:
					return VisitExclusiveOr(exp as BinaryExpression);
                case ExpressionType.ExclusiveOrAssign:
					return VisitExclusiveOrAssign(exp as BinaryExpression);
                case ExpressionType.Extension:
					return VisitExtension(exp);
                case ExpressionType.Goto:
					return VisitGoto(exp as GotoExpression);
                case ExpressionType.GreaterThan:
					return VisitGreaterThan(exp as BinaryExpression);
                case ExpressionType.GreaterThanOrEqual:
					return VisitGreaterThanOrEqual(exp as BinaryExpression);
                case ExpressionType.Increment:
					return VisitIncrement(exp as UnaryExpression);
                case ExpressionType.Index:
					return VisitIndex(exp as IndexExpression);
                case ExpressionType.Invoke:
					return VisitInvoke(exp as InvocationExpression);
                case ExpressionType.IsFalse:
					return VisitIsFalse(exp as UnaryExpression);
                case ExpressionType.IsTrue:
					return VisitIsTrue(exp as UnaryExpression);
                case ExpressionType.Label:
					return VisitLabel(exp as LabelExpression);
                case ExpressionType.Lambda:
					return VisitLambda(exp as LambdaExpression);
                case ExpressionType.LeftShift:
					return VisitLeftShift(exp as BinaryExpression);
                case ExpressionType.LeftShiftAssign:
					return VisitLeftShiftAssign(exp as BinaryExpression);
                case ExpressionType.LessThan:
					return VisitLessThan(exp as BinaryExpression);
                case ExpressionType.LessThanOrEqual:
					return VisitLessThanOrEqual(exp as BinaryExpression);
                case ExpressionType.ListInit:
					return VisitListInit(exp as ListInitExpression);
                case ExpressionType.Loop:
					return VisitLoop(exp as LoopExpression);
                case ExpressionType.MemberAccess:
					return VisitMemberAccess(exp as MemberExpression);
                case ExpressionType.MemberInit:
					return VisitMemberInit(exp as MemberInitExpression);
                case ExpressionType.Modulo:
					return VisitModulo(exp as BinaryExpression);
                case ExpressionType.ModuloAssign:
					return VisitModuloAssign(exp as BinaryExpression);
                case ExpressionType.Multiply:
					return VisitMultiply(exp as BinaryExpression);
                case ExpressionType.MultiplyAssign:
					return VisitMultiplyAssign(exp as BinaryExpression);
                case ExpressionType.MultiplyAssignChecked:
					return VisitMultiplyAssignChecked(exp as BinaryExpression);
                case ExpressionType.MultiplyChecked:
					return VisitMultiplyChecked(exp as BinaryExpression);
                case ExpressionType.Negate:
					return VisitNegate(exp as UnaryExpression);
                case ExpressionType.NegateChecked:
					return VisitNegateChecked(exp as BinaryExpression);
                case ExpressionType.New:
					return VisitNew(exp as NewExpression);
                case ExpressionType.NewArrayBounds:
					return VisitNewArrayBounds(exp as NewArrayExpression);
                case ExpressionType.NewArrayInit:
					return VisitNewArrayInit(exp as NewArrayExpression);
                case ExpressionType.Not:
					return VisitNot(exp as UnaryExpression);
                case ExpressionType.NotEqual:
					return VisitNotEqual(exp as BinaryExpression);
                case ExpressionType.OnesComplement:
					return VisitOnesComplement(exp as UnaryExpression);
                case ExpressionType.Or:
					return VisitOr(exp as BinaryExpression);
                case ExpressionType.OrAssign:
					return VisitOrAssign(exp as BinaryExpression);
                case ExpressionType.OrElse:
					return VisitOrElse(exp as BinaryExpression);
                case ExpressionType.Parameter:
					return VisitParameter(exp as ParameterExpression);
                case ExpressionType.PostDecrementAssign:
					return VisitPostDecrementAssign(exp as UnaryExpression);
                case ExpressionType.PostIncrementAssign:
					return VisitPostIncrementAssign(exp as UnaryExpression);
                case ExpressionType.Power:
					return VisitPower(exp as BinaryExpression);
                case ExpressionType.PowerAssign:
					return VisitPowerAssign(exp as BinaryExpression);
                case ExpressionType.PreDecrementAssign:
					return VisitPreDecrementAssign(exp as UnaryExpression);
                case ExpressionType.PreIncrementAssign:
					return VisitPreIncrementAssign(exp as UnaryExpression);
                case ExpressionType.Quote:
					return VisitQuote(exp as UnaryExpression);
                case ExpressionType.RightShift:
					return VisitRightShift(exp as BinaryExpression);
                case ExpressionType.RightShiftAssign:
					return VisitRightShiftAssign(exp as BinaryExpression);
                case ExpressionType.RuntimeVariables:
					return VisitRuntimeVariables(exp as RuntimeVariablesExpression);
                case ExpressionType.Subtract:
					return VisitSubtract(exp as BinaryExpression);
                case ExpressionType.SubtractAssign:
					return VisitSubtractAssign(exp as BinaryExpression);
                case ExpressionType.SubtractAssignChecked:
					return VisitSubtractAssignChecked(exp as BinaryExpression);
                case ExpressionType.SubtractChecked:
					return VisitSubtractChecked(exp as BinaryExpression);
                case ExpressionType.Switch:
					return VisitSwitch(exp as SwitchExpression);
                case ExpressionType.Throw:
					return VisitThrow(exp as UnaryExpression);
                case ExpressionType.Try:
					return VisitTry(exp as TryExpression);
                case ExpressionType.TypeAs:
					return VisitTypeAs(exp as UnaryExpression);
                case ExpressionType.TypeEqual:
					return VisitTypeEqual(exp as TypeBinaryExpression);
                case ExpressionType.TypeIs:
					return VisitTypeIs(exp as TypeBinaryExpression);
                case ExpressionType.UnaryPlus:
					return VisitUnaryPlus(exp as UnaryExpression);
                case ExpressionType.Unbox:
					return VisitUnbox(exp as UnaryExpression);
            }
            throw new NotSupportedException();
        }

        protected abstract T VisitUnbox(UnaryExpression node);
        protected abstract T VisitUnaryPlus(UnaryExpression node);
        protected abstract T VisitTypeIs(TypeBinaryExpression node);
        protected abstract T VisitTypeEqual(TypeBinaryExpression node);
        protected abstract T VisitTypeAs(UnaryExpression node);
        protected abstract T VisitTry(TryExpression node);
        protected abstract T VisitThrow(UnaryExpression node);
        protected abstract T VisitSwitch(SwitchExpression node);
        protected abstract T VisitSubtractChecked(BinaryExpression node);
        protected abstract T VisitSubtractAssignChecked(BinaryExpression node);
        protected abstract T VisitSubtractAssign(BinaryExpression node);
        protected abstract T VisitSubtract(BinaryExpression node);
        protected abstract T VisitRuntimeVariables(RuntimeVariablesExpression node);
        protected abstract T VisitRightShiftAssign(BinaryExpression node);
        protected abstract T VisitRightShift(BinaryExpression node);
        protected abstract T VisitQuote(UnaryExpression node);
        protected abstract T VisitPreIncrementAssign(UnaryExpression node);
        protected abstract T VisitPreDecrementAssign(UnaryExpression node);
        protected abstract T VisitPowerAssign(BinaryExpression node);
        protected abstract T VisitPower(BinaryExpression node);
        protected abstract T VisitPostIncrementAssign(UnaryExpression node);
        protected abstract T VisitPostDecrementAssign(UnaryExpression node);
        protected abstract T VisitParameter(ParameterExpression node);
        protected abstract T VisitOrElse(BinaryExpression node);
        protected abstract T VisitOrAssign(BinaryExpression node);
        protected abstract T VisitOr(BinaryExpression node);
        protected abstract T VisitOnesComplement(UnaryExpression node);
        protected abstract T VisitNotEqual(BinaryExpression node);
        protected abstract T VisitNot(UnaryExpression node);
        protected abstract T VisitNewArrayInit(NewArrayExpression node);
        protected abstract T VisitNewArrayBounds(NewArrayExpression node);
        protected abstract T VisitNew(NewExpression node);
        protected abstract T VisitNegateChecked(BinaryExpression node);
        protected abstract T VisitNegate(UnaryExpression node);
        protected abstract T VisitMultiplyChecked(BinaryExpression node);
        protected abstract T VisitMultiplyAssignChecked(BinaryExpression node);
        protected abstract T VisitMultiplyAssign(BinaryExpression node);
        protected abstract T VisitMultiply(BinaryExpression node);
        protected abstract T VisitModuloAssign(BinaryExpression node);
        protected abstract T VisitModulo(BinaryExpression node);
        protected abstract T VisitMemberInit(MemberInitExpression node);
        protected abstract T VisitMemberAccess(MemberExpression node);
        protected abstract T VisitLoop(LoopExpression node);
        protected abstract T VisitListInit(ListInitExpression node);
        protected abstract T VisitLessThanOrEqual(BinaryExpression node);
        protected abstract T VisitLessThan(BinaryExpression node);
        protected abstract T VisitLeftShiftAssign(BinaryExpression node);
        protected abstract T VisitLeftShift(BinaryExpression node);
        protected abstract T VisitLambda(LambdaExpression node);
        protected abstract T VisitLabel(LabelExpression node);
        protected abstract T VisitIsTrue(UnaryExpression node);
        protected abstract T VisitIsFalse(UnaryExpression node);
        protected abstract T VisitInvoke(InvocationExpression node);
        protected abstract T VisitIndex(IndexExpression node);
        protected abstract T VisitIncrement(UnaryExpression node);
        protected abstract T VisitGreaterThanOrEqual(BinaryExpression node);
        protected abstract T VisitGreaterThan(BinaryExpression node);
        protected abstract T VisitGoto(GotoExpression node);
        protected abstract T VisitExtension(Expression exp);
        protected abstract T VisitExclusiveOrAssign(BinaryExpression node);
        protected abstract T VisitExclusiveOr(BinaryExpression node);
        protected abstract T VisitEqual(BinaryExpression node);
        protected abstract T VisitDynamic(DynamicExpression node);
        protected abstract T VisitDivideAssign(BinaryExpression node);
        protected abstract T VisitDivide(BinaryExpression node);
        protected abstract T VisitDefault(DefaultExpression node);
        protected abstract T VisitDecrement(UnaryExpression node);
        protected abstract T VisitDebugInfo(ConstantExpression node);
        protected abstract T VisitConvertChecked(UnaryExpression node);
        protected abstract T VisitConvert(UnaryExpression node);
        protected abstract T VisitConstant(ConstantExpression node);
        protected abstract T VisitConditional(ConditionalExpression node);
        protected abstract T VisitCoalesce(BinaryExpression node);
        protected abstract T VisitCall(MethodCallExpression node);
        protected abstract T VisitBlock(BlockExpression node);
        protected abstract T VisitAssign(BinaryExpression node);
        protected abstract T VisitArrayLength(UnaryExpression node);
        protected abstract T VisitArrayIndex(BinaryExpression node);
        protected abstract T VisitAndAssign(BinaryExpression node);
        protected abstract T VisitAndAlso(BinaryExpression node);
        protected abstract T VisitAnd(BinaryExpression node);
        protected abstract T VisitAddChecked(BinaryExpression node);
        protected abstract T VisitAddAssignChecked(BinaryExpression node);
        protected abstract T VisitAddAssign(BinaryExpression node);
        protected abstract T VisitAdd(BinaryExpression node);
    }
}
