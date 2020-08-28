using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.LinqExpressions;

namespace WebAtoms.CoreJS.Utils
{
    public class LogicalCompare: SwitchExpression
    {
        public static Expression Compare(Expression left, Expression right, Esprima.Ast.BinaryOperator op)
        {
            switch(op)
            {
                case Esprima.Ast.BinaryOperator.StrictlyEqual:
                    return CompareStrictlyEqual(left, right);
                case Esprima.Ast.BinaryOperator.StricltyNotEqual:
                    return CompareStrictlyEqual(left, right);
                case Esprima.Ast.BinaryOperator.InstanceOf:
                    return ExpHelper.JSValue.InstanceOf(left, right);
                case Esprima.Ast.BinaryOperator.In:
                    return ExpHelper.JSValue.IsIn(left, right);
            }

            var caseNumber = Case<JSNumber>(l => 
                    Switch(right,
                        Case<JSNumber>(r => NumberCompare( ExpHelper.JSNumber.Value(l), ExpHelper.JSNumber.Value(r), op) ),
                        Default(NumberCompare(ExpHelper.JSNumber.Value(l), ExpHelper.JSValue.DoubleValue(right), op))));

            var caseString = Case<JSString>(l =>
                    Switch(right,
                        Case<JSString>(r => StringCompare(ExpHelper.JSString.Value(l), ExpHelper.JSString.Value(r), op)),
                        Default(StringCompare(ExpHelper.JSString.Value(l), ExpHelper.Object.ToString(right), op))));
            Expression defaultExp = ExpHelper.JSContext.False;

            switch(op)
            {
                case Esprima.Ast.BinaryOperator.Equal:
                    defaultExp = ExpHelper.JSBoolean.NewFromCLRBoolean(
                        Expression.ReferenceEqual(left,right));
                    break;
                case Esprima.Ast.BinaryOperator.NotEqual:
                    defaultExp = ExpHelper.JSBoolean.NewFromCLRBoolean(
                        Expression.ReferenceNotEqual(left, right));
                    break;
            }

            return Switch(
                left,
                caseNumber,
                caseString,
                Default(defaultExp)
                );
        }

        private static Expression StringCompare(Expression left, Expression right, Esprima.Ast.BinaryOperator op)
        {
            var Zero = Expression.Constant((int)0);
            Expression BooleanExpression() {
                
                switch (op)
                {
                    case Esprima.Ast.BinaryOperator.Equal:
                        return ExpHelper.String.Equals(left, right);
                    case Esprima.Ast.BinaryOperator.NotEqual:
                        return Expression.Not(ExpHelper.String.Equals(left, right));
                    case Esprima.Ast.BinaryOperator.Greater:
                        return Expression.GreaterThan(ExpHelper.String.Compare(left, right), Zero);
                    case Esprima.Ast.BinaryOperator.GreaterOrEqual:
                        return Expression.GreaterThanOrEqual(ExpHelper.String.Compare(left, right), Zero);
                    case Esprima.Ast.BinaryOperator.Less:
                        return Expression.LessThan(ExpHelper.String.Compare(left, right), Zero);
                    case Esprima.Ast.BinaryOperator.LessOrEqual:
                        return Expression.LessThanOrEqual(ExpHelper.String.Compare(left, right), Zero);
                    case Esprima.Ast.BinaryOperator.InstanceOf:
                        return Expression.Constant(false);
                    case Esprima.Ast.BinaryOperator.In:
                        return Expression.Constant(false);
                    case Esprima.Ast.BinaryOperator.LogicalAnd:
                        return Expression.Constant(false);
                    case Esprima.Ast.BinaryOperator.LogicalOr:
                        return Expression.Constant(false);
                }
                throw new NotImplementedException();
            }
            return ExpHelper.JSBoolean.NewFromCLRBoolean(BooleanExpression());
        }


        private static Expression NumberCompare(Expression left, Expression right, Esprima.Ast.BinaryOperator op) {
            Expression BooleanExpression()
            {
                switch (op)
                {
                    case Esprima.Ast.BinaryOperator.Equal:
                        return Expression.Equal(left, right);
                    case Esprima.Ast.BinaryOperator.NotEqual:
                        return Expression.NotEqual(left, right);
                    case Esprima.Ast.BinaryOperator.Greater:
                        return Expression.GreaterThan(left, right);
                    case Esprima.Ast.BinaryOperator.GreaterOrEqual:
                        return Expression.GreaterThanOrEqual(left, right);
                    case Esprima.Ast.BinaryOperator.Less:
                        return Expression.LessThan(left, right);
                    case Esprima.Ast.BinaryOperator.LessOrEqual:
                        return Expression.LessThanOrEqual(left, right);
                    //case Esprima.Ast.BinaryOperator.StrictlyEqual:
                    //    break;
                    //case Esprima.Ast.BinaryOperator.StricltyNotEqual:
                    //    break;
                    //case Esprima.Ast.BinaryOperator.InstanceOf:
                    //    break;
                    //case Esprima.Ast.BinaryOperator.In:
                    //    break;
                    case Esprima.Ast.BinaryOperator.LogicalAnd:
                        return Expression.And(left, right);
                    case Esprima.Ast.BinaryOperator.LogicalOr:
                        return Expression.Or(left, right);
                }
                throw new NotImplementedException();
            }

            // The reason we are returning JSBoolean is to avoid recreating
            // as this can be used with another expression like instance of or in
            return ExpHelper.JSBoolean.NewFromCLRBoolean(BooleanExpression());
        }

        static Expression CompareStrictlyEqual(Expression leftExp, Expression rightExp, bool inverse = false)
        {
            var True = inverse ? ExpHelper.JSContext.False : ExpHelper.JSContext.True;
            var False = inverse ? ExpHelper.JSContext.True : ExpHelper.JSContext.False;

            var caseUndefined =
                Case<JSUndefined>(left => {
                    return Switch(rightExp,
                        Case<JSUndefined>(right => True),
                        Default(False)
                    );
                    });

            var caseNull =
                Case<JSNull>(left =>
                {
                    return Switch(rightExp,
                        Case<JSNull>(x => True),
                        Default(False));
                });

            var caseNumber =
                Case<JSNumber>(left =>
                {
                    return Switch(rightExp,
                    Case<JSNumber>(right =>
                        Expression.Condition(
                            Expression.Equal(ExpHelper.JSNumber.Value(left), ExpHelper.JSNumber.Value(right)),
                            True,
                            False)
                    ),
                    Default(False));
                });

            var caseString =
                Case<JSString>(left =>
                {
                    return Switch(rightExp,
                    Case<JSString>(right =>
                        Expression.Condition(
                            ExpHelper.String.Equals(ExpHelper.JSString.Value(left), ExpHelper.JSString.Value(right)),
                            True,
                            False)
                    ),
                    Default(False));
                });

            var lastCase =
                Expression.Condition(
                    ExpHelper.Object.RefEquals(leftExp, rightExp),
                    True,
                    False);

            return Switch(leftExp,
                    caseUndefined,
                    caseNull,
                    caseNumber,
                    caseString,
                    Default(lastCase)
                );

            //var leftBoolean = Expression.Variable(typeof(JSBoolean));
            //var rightBoolean = Expression.Variable(typeof(JSBoolean));
            
            //var leftNumber = Expression.Variable(typeof(JSNumber));
            //var rightNumber = Expression.Variable(typeof(JSNumber));
            
            //var leftString = Expression.Variable(typeof(JSString));
            //var rightString = Expression.Variable(typeof(JSString));

            //var nullBoolean = Expression.Constant(null, typeof(JSBoolean));
            //var nullNumber = Expression.Constant(null, typeof(JSNumber));
            //var nullString = Expression.Constant(null, typeof(JSString));

            //var compareBoolean =
            //    Expression.Block(new ParameterExpression[] { rightBoolean },
            //    Expression.Assign(rightBoolean, Expression.TypeAs(right, typeof(JSBoolean))),
            //    Expression.Condition(
            //        Expression.NotEqual(nullBoolean, rightBoolean),
            //        Expression.Condition(
            //            Expression.Equal( ExpHelper.JSBoolean.Value(leftBoolean), ExpHelper.JSBoolean.Value(rightBoolean) ),
            //            ExpHelper.JSContext.True,
            //            ExpHelper.JSContext.False),
            //        ExpHelper.JSContext.False
            //    ));

            //var compareNumber =
            //    Expression.Block(new ParameterExpression[] { rightNumber },
            //        Expression.Assign(rightNumber, Expression.TypeAs(right, typeof(JSNumber))),
            //        Expression.Condition(
            //            Expression.NotEqual(nullNumber,rightNumber),
            //            Expression.Condition(
            //                Expression.Equal(ExpHelper.JSNumber.Value(leftNumber), ExpHelper.JSNumber.Value(rightNumber)),
            //                ExpHelper.JSContext.True,
            //                ExpHelper.JSContext.False),
            //            ExpHelper.JSContext.False
            //    ));

            //var compareString =
            //    Expression.Block(new ParameterExpression[] { rightString },
            //        Expression.Assign(rightString, Expression.TypeAs(right, typeof(JSString))),
            //        Expression.Condition(
            //            Expression.NotEqual(nullString, rightString),
            //            Expression.Condition(
            //                ExpHelper.String.Equals(ExpHelper.JSString.Value(leftString), ExpHelper.JSString.Value(rightString)),
            //                ExpHelper.JSContext.True,
            //                ExpHelper.JSContext.False),
            //            ExpHelper.JSContext.False
            //    ));

            //return Expression.Condition(
            //            Expression.And(Expression.TypeIs(left, typeof(JSNull)), Expression.TypeIs(right, typeof(JSNull))),
            //            ExpHelper.JSContext.True,

            //            Expression.Condition(
            //                Expression.And(Expression.TypeIs(left, typeof(JSUndefined)), Expression.TypeIs(right, typeof(JSUndefined))),
            //                ExpHelper.JSContext.True,

            //                Expression.Block(
            //                    new ParameterExpression[] { leftNumber },
            //                    Expression.Assign(leftNumber, Expression.TypeAs(left, typeof(JSNumber))),
            //                    Expression.Condition(
            //                        Expression.NotEqual(nullNumber, leftNumber),
            //                        compareNumber,
            //                        Expression.Block(
            //                            new ParameterExpression[] { leftString },
            //                            Expression.Assign(leftString, Expression.TypeAs(left, typeof(JSString))),
            //                            Expression.Condition(
            //                                Expression.NotEqual(nullString, leftString),
            //                                compareString,
            //                                Expression.Block(
            //                                    new ParameterExpression[] { leftBoolean },
            //                                    Expression.Assign(leftBoolean, Expression.TypeAs(left, typeof(JSBoolean))),
            //                                    Expression.Condition(
            //                                            Expression.NotEqual(nullBoolean, leftBoolean),
            //                                            compareBoolean,
            //                                            ExpHelper.JSContext.False
            //                                        )
            //                                    )
            //                                )
            //                            )
            //                        )
            //                    )

            //            )
            //        )
            //    ;
        }
    }
}
