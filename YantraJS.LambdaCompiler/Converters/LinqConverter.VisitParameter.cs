using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Converters
{

    public partial class LinqConverter
    {
        protected override YExpression VisitAdd(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Add, Visit(node.Right));
        }

        protected override YExpression VisitAddAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitAddAssignChecked(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitAddChecked(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitAnd(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.BitwiseAnd, Visit(node.Right));
        }

        protected override YExpression VisitAndAlso(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.BooleanAnd, Visit(node.Right));
        }

        protected override YExpression VisitAndAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitArrayIndex(BinaryExpression node)
        {
            return YExpression.ArrayIndex(Visit(node.Left), Visit(node.Right));
        }

        protected override YExpression VisitArrayLength(UnaryExpression node)
        {
            return YExpression.ArrayLength(Visit(node.Operand));
        }

        protected override YExpression VisitAssign(BinaryExpression node)
        {
            if (node.Conversion != null)
                throw new NotSupportedException();
            return YExpression.Assign(Visit(node.Left), Visit(node.Right));

        }

        protected override YExpression VisitConditional(ConditionalExpression node)
        {
            return YExpression.Conditional(Visit(node.Test), Visit(node.IfTrue), Visit(node.IfFalse));
        }

        protected override YExpression VisitConstant(ConstantExpression node)
        {
            return YExpression.Constant(node.Value, node.Type);
        }

        protected override YExpression VisitConvert(UnaryExpression node)
        {
            return YExpression.Convert(Visit(node.Operand), node.Type, true);
        }

        protected override YExpression VisitConvertChecked(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitDebugInfo(ConstantExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitDecrement(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitDefault(DefaultExpression node)
        {
            if (node.Type == typeof(void))
            {
                return YExpression.Empty;
            }
            return YExpression.Null;

        }

        protected override YExpression VisitDivide(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Divide, Visit(node.Right));
        }

        protected override YExpression VisitDivideAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitDynamic(DynamicExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitEqual(BinaryExpression node)
        {
            return YExpression.Equal(Visit(node.Left), Visit(node.Right));
        }

        protected override YExpression VisitExclusiveOr(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Xor, Visit(node.Right));
        }

        protected override YExpression VisitExclusiveOrAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitExtension(Expression exp)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitGoto(GotoExpression node)
        {
            switch (node.Kind)
            {
                case GotoExpressionKind.Break:
                case GotoExpressionKind.Continue:
                case GotoExpressionKind.Goto:
                    return YExpression.GoTo(labels[node.Target], Visit(node.Value));
                case GotoExpressionKind.Return:
                    return YExpression.Return(labels[node.Target], Visit(node.Value));
                default:
                    throw new NotImplementedException();
            }

        }

        protected override YExpression VisitGreaterThan(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Greater, Visit(node.Right));
        }

        protected override YExpression VisitGreaterThanOrEqual(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.GreaterOrEqual, Visit(node.Right));
        }

        protected override YExpression VisitIncrement(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitIndex(IndexExpression node)
        {
            return YExpression.Index(Visit(node.Object), node.Indexer, VisitList(node.Arguments));
        }

        protected override YExpression VisitInvoke(InvocationExpression node)
        {
            return YExpression.Invoke(Visit(node.Expression), VisitList(node.Arguments));
        }

        protected override YExpression VisitIsFalse(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitIsTrue(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitLabel(LabelExpression node)
        {
            return YExpression.Label(labels[node.Target], Visit(node.DefaultValue));
        }

        protected override YExpression VisitLeftShift(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.LeftShift, Visit(node.Right));
        }

        protected override YExpression VisitLeftShiftAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitLessThan(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Less, Visit(node.Right));
        }

        protected override YExpression VisitLessThanOrEqual(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.LessOrEqual, Visit(node.Right));
        }

        protected override YExpression VisitListInit(ListInitExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitLoop(LoopExpression node)
        {
            return YExpression.Loop(Visit(node.Body), labels[node.BreakLabel], node.ContinueLabel != null ? labels[node.ContinueLabel] : null);
        }

        protected override YExpression VisitMemberAccess(MemberExpression node)
        {
            if (node.Member is FieldInfo field)
                return YExpression.Field(Visit(node.Expression), field);
            if (node.Member is PropertyInfo property)
                return YExpression.Property(Visit(node.Expression), property);
            throw new NotImplementedException();
        }

        protected override YExpression VisitMemberInit(MemberInitExpression node)
        {
            return YExpression.MemberInit(Visit(node.NewExpression) as YNewExpression,
                node.Bindings.Select(m => Visit(m)).ToArray());
        }

        protected override YExpression VisitModulo(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Mod, Visit(node.Right));
        }

        protected override YExpression VisitModuloAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitMultiply(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Multipley, Visit(node.Right));
        }

        protected override YExpression VisitMultiplyAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitMultiplyAssignChecked(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitMultiplyChecked(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitNegate(UnaryExpression node)
        {
            return YExpression.Negative(Visit(node.Operand));
        }

        protected override YExpression VisitNegateChecked(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitNew(NewExpression node)
        {
            return YExpression.New(node.Constructor, VisitList(node.Arguments));
        }

        protected override YExpression VisitNewArrayBounds(NewArrayExpression node)
        {
            return YExpression.NewArrayBounds(node.Type.GetElementType(), Visit(node.Expressions.First()));
        }

        protected override YExpression VisitNewArrayInit(NewArrayExpression node)
        {
            return YExpression.NewArray(node.Type.GetElementType(), VisitList(node.Expressions));
        }

        protected override YExpression VisitNot(UnaryExpression node)
        {
            return YExpression.Not(Visit(node.Operand));
        }

        protected override YExpression VisitNotEqual(BinaryExpression node)
        {
            return YExpression.NotEqual(Visit(node.Left), Visit(node.Right));
        }

        protected override YExpression VisitOnesComplement(UnaryExpression node)
        {
            return YExpression.OnesComplement(Visit(node.Operand));
        }

        protected override YExpression VisitOr(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.BitwiseOr, Visit(node.Right));
        }

        protected override YExpression VisitOrAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitOrElse(BinaryExpression node)
        {
            return YExpression.OrElse(Visit(node.Left), Visit(node.Right));
        }

        protected override YExpression VisitParameter(ParameterExpression node)
        {
            return parameters[node as ParameterExpression];
        }

        protected override YExpression VisitPostDecrementAssign(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitPostIncrementAssign(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitPower(BinaryExpression node)
        {
            var m = typeof(Math).GetMethod(nameof(Math.Pow));
            // return YExpression.Binary(Visit(node.Left), YOperator.Power, Visit(node.Right));
            var left = Visit(node.Left);
            var right = Visit(node.Right);
            left = left.Type == typeof(double) ? left : YExpression.Convert(left, typeof(double));
            right = right.Type == typeof(double) ? right: YExpression.Convert(right, typeof(double));
            return YExpression.Call(null, m, left, right);
        }

        protected override YExpression VisitPowerAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitPreDecrementAssign(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitPreIncrementAssign(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitQuote(UnaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitRightShift(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.RightShift, Visit(node.Right));
        }

        protected override YExpression VisitRightShiftAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitSubtract(BinaryExpression node)
        {
            return YExpression.Binary(Visit(node.Left), YOperator.Subtract, Visit(node.Right));
        }

        protected override YExpression VisitSubtractAssign(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitSubtractAssignChecked(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitSubtractChecked(BinaryExpression node)
        {
            throw new NotImplementedException();
        }

        protected override YExpression VisitSwitch(SwitchExpression node)
        {
            var cases = node.Cases.Select(x =>
                YExpression.SwitchCase(Visit(x.Body),
                x.TestValues.Select(Visit).ToArray()
            )).ToArray();

            return YExpression.Switch(
                Visit(node.SwitchValue),
                Visit(node.DefaultBody),
                node.Comparison,
                cases);

        }

        protected override YExpression VisitThrow(UnaryExpression node)
        {
            return YExpression.Throw(Visit(node.Operand));
        }

        protected override YExpression VisitTry(TryExpression node)
        {
            YCatchBody cb = null;
            if (node.Handlers.Count > 0)
            {
                var first = node.Handlers.First();
                cb = first.Variable != null
                    ? YExpression.Catch(parameters[first.Variable], Visit(first.Body))
                    : YExpression.Catch(Visit(first.Body));
            }
            return YExpression.TryCatchFinally(Visit(node.Body), cb, Visit(node.Finally));
        }

        protected override YExpression VisitTypeAs(UnaryExpression node)
        {
            return YExpression.TypeAs(Visit(node.Operand), node.Type);
        }

        protected override YExpression VisitTypeEqual(TypeBinaryExpression node)
        {
            return YExpression.TypeIs(Visit(node.Expression), node.TypeOperand);
        }

        protected override YExpression VisitTypeIs(TypeBinaryExpression node)
        {
            return YExpression.TypeIs(Visit(node.Expression), node.TypeOperand);
        }

        protected override YExpression VisitUnaryPlus(UnaryExpression node)
        {
            return Visit(node.Operand);
        }

        protected override YExpression VisitUnbox(UnaryExpression node)
        {
            throw new NotImplementedException();
        }
    }
}
