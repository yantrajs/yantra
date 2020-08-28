using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.LinqExpressions;
using AssignmentOperator = Esprima.Ast.AssignmentOperator;

namespace WebAtoms.CoreJS.Utils
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
                        Expression.Block(new ParameterExpression[] { bp },
                            @case.TrueCase(bp)
                        ),
                        condition,
                        typeof(JSValue)
                        );
                    continue;
                }

                var nbt = Expression.Constant(null, @case.Type);
                condition = Expression.Block(new ParameterExpression[] { bp },
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


        public static Expression Assign(Expression left, Expression right, AssignmentOperator assignmentOperator)
        {



            switch(assignmentOperator)
            {
                case AssignmentOperator.Assign:
                    return Expression.Assign(left, right);
                case AssignmentOperator.PlusAssign:
                    return Expression.Assign(left, ExpHelper.JSValue.Add(left,right));
            }

            var leftDouble = ExpHelper.JSValue.DoubleValue(left);
            var rightDouble = ExpHelper.JSValue.DoubleValue(right);

            var leftInt = Expression.Convert(leftDouble, typeof(int));
            var rightInt = Expression.Convert(rightDouble, typeof(int));

            var rightUInt = Expression.Convert(rightDouble, typeof(uint));

            // convert to double...
            switch (assignmentOperator)
            {
                case AssignmentOperator.MinusAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.Add(leftDouble, rightDouble)));
                case AssignmentOperator.TimesAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.Multiply(leftDouble, rightDouble)));
                case AssignmentOperator.DivideAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.Divide(leftDouble, rightDouble)));
                case AssignmentOperator.ModuloAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.Modulo(leftDouble, rightDouble)));
                case AssignmentOperator.BitwiseAndAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.And(leftInt, rightInt)));
                case AssignmentOperator.BitwiseOrAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.Or(leftInt, rightInt)));
                case AssignmentOperator.BitwiseXOrAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.ExclusiveOr(leftInt, rightInt)));
                case AssignmentOperator.LeftShiftAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.LeftShift(leftInt, rightInt)));
                case AssignmentOperator.RightShiftAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.RightShift(leftInt, rightInt)));
                case AssignmentOperator.UnsignedRightShiftAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.RightShift(leftInt, rightUInt)));
                case AssignmentOperator.ExponentiationAssign:
                    return Expression.Assign(left, ExpHelper.JSNumber.New(Expression.Power(leftInt, rightInt)));
            }

            throw new NotSupportedException();
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
        public static Expression Operation(Expression left, Expression right, Esprima.Ast.BinaryOperator op)
        {
            var leftDouble = ExpHelper.JSValue.DoubleValue(left);
            var rightDouble = ExpHelper.JSValue.DoubleValue(right);

            var leftInt = Expression.Convert(leftDouble, typeof(int));
            var rightInt = Expression.Convert(rightDouble, typeof(int));

            var rightUInt = Expression.Convert(rightDouble, typeof(uint));

            switch (op)
            {
                case Esprima.Ast.BinaryOperator.StrictlyEqual:
                    return ExpHelper.JSValue.StrictEquals(left, right);
                case Esprima.Ast.BinaryOperator.StricltyNotEqual:
                    return ExpHelper.JSValue.NotStrictEquals(left, right);
                case Esprima.Ast.BinaryOperator.InstanceOf:
                    return ExpHelper.JSValue.InstanceOf(left, right);
                case Esprima.Ast.BinaryOperator.In:
                    return ExpHelper.JSValue.IsIn(left, right);
                case Esprima.Ast.BinaryOperator.Plus:
                    return ExpHelper.JSValue.Add(left, right);
                case Esprima.Ast.BinaryOperator.Minus:
                    return ExpHelper.JSNumber.New(Expression.Subtract(leftDouble, rightDouble));
                case Esprima.Ast.BinaryOperator.Times:
                    return ExpHelper.JSNumber.New(Expression.Multiply(leftDouble, rightDouble));
                case Esprima.Ast.BinaryOperator.Divide:
                    return ExpHelper.JSNumber.New(Expression.Divide(leftDouble, rightDouble));
                case Esprima.Ast.BinaryOperator.Modulo:
                    return ExpHelper.JSNumber.New(Expression.Modulo(leftDouble, rightDouble));
                case Esprima.Ast.BinaryOperator.Equal:
                    return ExpHelper.JSValue.Equals(left, right);
                case Esprima.Ast.BinaryOperator.NotEqual:
                    return ExpHelper.JSValue.NotEquals(left, right);
                case Esprima.Ast.BinaryOperator.Greater:
                    return ExpHelper.JSValue.Greater(left, right);
                case Esprima.Ast.BinaryOperator.GreaterOrEqual:
                    return ExpHelper.JSValue.GreaterOrEqual(left, right);
                case Esprima.Ast.BinaryOperator.Less:
                    return ExpHelper.JSValue.Less(left, right);
                case Esprima.Ast.BinaryOperator.LessOrEqual:
                    return ExpHelper.JSValue.LessOrEqual(left, right);
                case Esprima.Ast.BinaryOperator.BitwiseAnd:
                    return ExpHelper.JSNumber.New(Expression.And(leftInt, rightInt));
                case Esprima.Ast.BinaryOperator.BitwiseOr:
                    return ExpHelper.JSNumber.New(Expression.Or(leftInt, rightInt));
                case Esprima.Ast.BinaryOperator.BitwiseXOr:
                    return ExpHelper.JSNumber.New(Expression.ExclusiveOr(leftInt, rightInt));
                case Esprima.Ast.BinaryOperator.LeftShift:
                    return ExpHelper.JSNumber.New(Expression.LeftShift(leftInt, rightInt));
                case Esprima.Ast.BinaryOperator.RightShift:
                    return ExpHelper.JSNumber.New(Expression.RightShift(leftInt, rightInt));
                case Esprima.Ast.BinaryOperator.UnsignedRightShift:
                    return ExpHelper.JSNumber.New(Expression.RightShift(leftInt, rightUInt));
                case Esprima.Ast.BinaryOperator.LogicalAnd:
                    return ExpHelper.JSValue.LogicalAnd(left, right);
                case Esprima.Ast.BinaryOperator.LogicalOr:
                    return ExpHelper.JSValue.LogicalOr(left, right);
                case Esprima.Ast.BinaryOperator.Exponentiation:
                    return ExpHelper.JSNumber.New(Expression.Power(leftDouble, rightDouble));
            }
            throw new NotImplementedException();
        }





    }
}
