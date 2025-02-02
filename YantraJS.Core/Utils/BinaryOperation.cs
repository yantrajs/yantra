using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.FastParser;
using YantraJS.ExpHelper;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.LambdaGen;


namespace YantraJS.Utils
{
    public delegate Expression CaseExpression(ParameterExpression pe);

    public class SwitchExpression
    {
        protected static TypeCheckCase Case<T>(in CaseExpression e)
        {
            return new TypeCheckCase
            {
                Type = typeof(T),
                TrueCase = e
            };
        }

        protected static TypeCheckCase Case(Type type, CaseExpression e)
        {
            return new TypeCheckCase
            {
                Type = type,
                TrueCase = e
            };
        }

        protected static TypeCheckCase Default(Expression e)
        {
            return new TypeCheckCase
            {
                Type = null,
                TrueCase = (a) => e
            };
        }

        public class TypeCheckCase
        {
            public Type Type { get; set; }

            public CaseExpression TrueCase { get; set; }

        }

        protected static Expression
            Switch(Expression right,
                    params TypeCheckCase[] cases)
        {

            var defaultCase = cases.First(x => x.Type == null);
            var allCases = cases.Where(x => x.Type != null);

            Expression condition = defaultCase.TrueCase(null);
            foreach (var @case in allCases)
            {
                var bp = Expression.Parameter(@case.Type);

                if (@case.Type.IsValueType)
                {
                    condition = Expression.Condition(
                        Expression.TypeIs(right, @case.Type),
                        Expression.Block(bp.AsSequence(),
                            @case.TrueCase(bp)
                        ),
                        condition,
                        typeof(JSValue)
                        );
                    continue;
                }

                var nbt = Expression.Constant(null, @case.Type);
                condition = Expression.Block(bp.AsSequence(),
                    Expression.Assign(bp, Expression.TypeAs(right, @case.Type)),
                    Expression.Condition(
                        Expression.NotEqual(nbt, bp),
                        @case.TrueCase(bp),
                        condition,
                        typeof(JSValue)
                    ));
            }
            return condition;
        }
    }

    public class BinaryOperation: SwitchExpression
    {

        public static Expression Assign(Expression left, Expression right, TokenTypes assignmentOperator)
        {

            var oneF = Expression.Constant(0x1F);

            switch (assignmentOperator)
            {
                case TokenTypes.Assign:
                    return Assign(left, right);
                case TokenTypes.AssignAdd:
                    return Assign(left, ExpHelper.JSValueBuilder.Add(left, right));
            }

            var leftDouble = ExpHelper.JSValueBuilder.DoubleValue(left);
            var rightDouble = ExpHelper.JSValueBuilder.DoubleValue(right);

            var leftInt = JSValueBuilder.IntValue(left);
            var rightInt = JSValueBuilder.IntValue(right);
            var leftUInt = Expression.Convert(leftDouble, typeof(uint));
           

            var rightUInt = Expression.Convert(rightDouble, typeof(uint));

            // convert to double...
            switch (assignmentOperator)
            {
                case TokenTypes.AssignSubtract:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.Subtract(leftDouble, rightDouble)));
                case TokenTypes.AssignMultiply:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.Multiply(leftDouble, rightDouble)));
                case TokenTypes.AssignDivide:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.Divide(leftDouble, rightDouble)));
                case TokenTypes.AssignMod:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.Modulo(leftDouble, rightDouble)));
                case TokenTypes.AssignBitwideAnd:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.And(leftInt, rightInt)));
                case TokenTypes.AssignBitwideOr:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.Or(leftInt, rightInt)));
                case TokenTypes.AssignXor:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.ExclusiveOr(leftInt, rightInt)));
                case TokenTypes.AssignLeftShift:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.LeftShift(leftInt, rightInt)));
                case TokenTypes.AssignRightShift:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.RightShift(leftInt,Expression.And( rightInt, oneF))));
                case TokenTypes.AssignUnsignedRightShift:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.RightShift(leftInt, rightInt)));
                case TokenTypes.AssignPower:
                    return Assign(left, ExpHelper.JSNumberBuilder.New(Expression.Power(leftDouble, rightDouble)));
                case TokenTypes.AssignCoalesce:
                    return Assign(left, JSValueBuilder.Coalesce(left, right));
            }

            throw new NotSupportedException();
        }

        private static Expression Assign(Expression left, Expression right)
        {
            return ExpHelper.JSValueExtensionsBuilder.Assign(left, right);
        }

        #region Add

        //public static Expression Add(Expression leftExp, Expression right)
        //{
        //    object obj = 4;

        //    var undefined = Expression.Constant("undefined");
        //    var @null = Expression.Constant("null");
        //    var nan = ExpHelper.JSContext.NaN;
        //    var zero = ExpHelper.JSContext.Zero;
        //    CaseExpression caseUndefined = (left) =>
        //        Switch(right,
        //        Case<JSUndefined>(x => nan),
        //        Case<JSNumber>(x => nan),
        //        Case<double>(x => nan),
        //        Case<string>(x => ExpHelper.JSString.ConcatBasicStrings(undefined, x)),
        //        Default(ExpHelper.JSString.ConcatBasicStrings(undefined, ExpHelper.Object.ToString(right)))
        //        );

        //    CaseExpression caseNull = (left) =>
        //        Switch(right,
        //        Case<JSUndefined>(x => nan),
        //        Case<JSNull>(x => zero),
        //        Case<JSNumber>(x => right),
        //        Case<double>(x => ExpHelper.JSNumber.New(x)),
        //        Case<string>(x => ExpHelper.JSString.ConcatBasicStrings(@null, x)),
        //        Default(ExpHelper.JSString.ConcatBasicStrings(@null, ExpHelper.Object.ToString(right)))
        //        );

        //    // string case avoids toString  of JSString by accessing value directly...
        //    CaseExpression caseJSString = (left) =>
        //        Switch(right,
        //        Case<JSUndefined>(x => ExpHelper.JSString.ConcatBasicStrings(ExpHelper.JSString.Value(left), undefined)),
        //        Case<JSNull>(x => ExpHelper.JSString.ConcatBasicStrings(ExpHelper.JSString.Value(left), @null)),
        //        Case<JSNumber>(x => ExpHelper.JSString.ConcatBasicStrings(
        //            ExpHelper.JSString.Value(left),
        //            ExpHelper.Object.ToString(ExpHelper.JSNumber.Value(x)))),
        //        Case<double>(x => ExpHelper.JSString.ConcatBasicStrings(
        //            ExpHelper.JSString.Value(left),
        //            ExpHelper.Object.ToString(x))),
        //        Case<string>(x => ExpHelper.JSString.ConcatBasicStrings(@null, x)),
        //        Default(ExpHelper.JSString.ConcatBasicStrings(@null, ExpHelper.Object.ToString(right)))
        //        );

        //    // JSNumber is the most complicated one, and will be too big, so we will
        //    // call a method on it ..
        //    // also it should be the first case as most likely we will add numbers and strings...
        //    CaseExpression caseJSNumber = (left) =>
        //        ExpHelper.JSNumber.AddValue(left, right);

        //    var StringAdd =
        //        ExpHelper.JSString.ConcatBasicStrings(
        //                ExpHelper.Object.ToString(leftExp),
        //                ExpHelper.Object.ToString(right)
        //                );

        //    return Switch(leftExp,
        //            Case<JSNumber>(caseJSNumber),
        //            Case<JSString>(caseJSString),
        //            Case<JSUndefined>(caseUndefined),
        //            Case<JSNull>(caseNull),
        //            Default(StringAdd)
        //        );


        //}
        #endregion
        public static Expression Operation(Expression left, Expression right, TokenTypes op)
        {
            //var leftDouble = ExpHelper.JSValueBuilder.DoubleValue(left);
            //var rightDouble = ExpHelper.JSValueBuilder.DoubleValue(right);

            //var leftInt = JSValueBuilder.IntValue(left);
            //var rightInt = JSValueBuilder.IntValue(right);

            //var rightUInt = Expression.Convert(rightDouble, typeof(uint));

            //var oneF = Expression.Constant(0x1F);

            switch (op)
            {
                case TokenTypes.Equal:
                    return ExpHelper.JSValueBuilder.Equals(left, right);
                case TokenTypes.NotEqual:
                    return ExpHelper.JSValueBuilder.NotEquals(left, right);
                case TokenTypes.StrictlyEqual:
                    return ExpHelper.JSValueBuilder.StrictEquals(left, right);
                case TokenTypes.StrictlyNotEqual:
                    return ExpHelper.JSValueBuilder.NotStrictEquals(left, right);


                case TokenTypes.InstanceOf:
                    return ExpHelper.JSValueExtensionsBuilder.InstanceOf(left, right);
                case TokenTypes.In:
                    return ExpHelper.JSValueExtensionsBuilder.IsIn(left, right);
                case TokenTypes.Plus:
                    return ExpHelper.JSValueBuilder.Add(left, right);
                case TokenTypes.Minus:
                    // return ExpHelper.JSNumberBuilder.New(Expression.Subtract(leftDouble, rightDouble));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a,b) => a.Subtract(b), right);
                case TokenTypes.Multiply:
                    // return ExpHelper.JSNumberBuilder.New(Expression.Multiply(leftDouble, rightDouble));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.Multiply(b), right);
                case TokenTypes.Divide:
                    // return ExpHelper.JSNumberBuilder.New(Expression.Divide(leftDouble, rightDouble));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.Divide(b), right);
                case TokenTypes.Mod:
                    // return ExpHelper.JSNumberBuilder.New(Expression.Modulo(leftDouble, rightDouble));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.Modulo(b), right);
                case TokenTypes.Greater:
                    return ExpHelper.JSValueBuilder.Greater(left, right);
                case TokenTypes.GreaterOrEqual:
                    return ExpHelper.JSValueBuilder.GreaterOrEqual(left, right);
                case TokenTypes.Less:
                    return ExpHelper.JSValueBuilder.Less(left, right);
                case TokenTypes.LessOrEqual:
                    return ExpHelper.JSValueBuilder.LessOrEqual(left, right);
                case TokenTypes.BitwiseAnd:
                    // return ExpHelper.JSNumberBuilder.New(Expression.And(leftInt, rightInt));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.BitwiseAnd(b), right);
                case TokenTypes.BitwiseOr:
                    // return ExpHelper.JSNumberBuilder.New(Expression.Or(leftInt, rightInt));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.BitwiseOr(b), right);
                case TokenTypes.Xor:
                    // return ExpHelper.JSNumberBuilder.New(Expression.ExclusiveOr(leftInt, rightInt));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.BitwiseXor(b), right);
                case TokenTypes.LeftShift:
                    // return ExpHelper.JSNumberBuilder.New(Expression.LeftShift(leftInt, rightInt));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.LeftShift(b), right);
                case TokenTypes.RightShift:
                    // return ExpHelper.JSNumberBuilder.New(Expression.RightShift(leftInt,Expression.And( rightInt, oneF)));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.RightShift(b), right);
                case TokenTypes.UnsignedRightShift:
                    //return ExpHelper.JSNumberBuilder.New(
                    //    Expression.UnsignedRightShift(
                    //        JSValueBuilder.UIntValue(left) , rightInt));
                    return left.CallExpression<JSValue, JSValue, JSValue>(() => (a, b) => a.UnsignedRightShift(b), right);
                case TokenTypes.BooleanAnd:
                    return ExpHelper.JSValueBuilder.LogicalAnd(left, right);
                case TokenTypes.BooleanOr:
                    return ExpHelper.JSValueBuilder.LogicalOr(left, right);
                case TokenTypes.Power:

                    return ExpHelper.JSValueBuilder.Power(left, right);
                case TokenTypes.Coalesce:
                    return ExpHelper.JSValueExtensionsBuilder.Coalesce(left, right);
            }
            return null;
        }

    }
}
