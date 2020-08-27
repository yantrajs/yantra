using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.LinqExpressions;

namespace WebAtoms.CoreJS.Utils
{
    public static class LogicalCompare
    {

        public static Expression Compare(Expression left, Expression right, Esprima.Ast.BinaryOperator op)
        {
            if (op == Esprima.Ast.BinaryOperator.StrictlyEqual)
            {
                return CompareStrictlyEqual(left, right);
            }

            var n = Expression.Constant(null, typeof(JSNumber));
            var ns = Expression.Constant(null, typeof(JSString));
            var l = Expression.Variable(typeof(JSNumber));
            var r = Expression.Variable(typeof(JSNumber));
            var ls = Expression.Variable(typeof(JSString));
            var rs = Expression.Variable(typeof(JSString));
            return Expression.Block(new ParameterExpression[] { l, r }, 
                Expression.Assign(l, Expression.TypeAs(left, typeof(JSNumber))),
                Expression.Assign(r, Expression.TypeAs(right, typeof(JSNumber))),
                Expression.Condition(
                    Expression.And(Expression.NotEqual(n, l), Expression.NotEqual(n, r)),
                    NumberCompare( ExpHelper.JSNumber.Value(l), ExpHelper.JSNumber.Value(r), op),
                    Expression.Block(
                        new ParameterExpression[] { ls, rs },
                        Expression.Assign(ls, Expression.TypeAs(left, typeof(JSString))),
                        Expression.Assign(rs, Expression.TypeAs(right, typeof(JSString))),
                        StringCompare(
                            Expression.Condition(Expression.NotEqual(ns,ls), ExpHelper.JSString.Value(ls), ExpHelper.Object.ToString(left)),
                            Expression.Condition(Expression.NotEqual(ns, rs), ExpHelper.JSString.Value(rs), ExpHelper.Object.ToString(left)),
                            op)
                        )
                ));
        }

        private static Expression NumberCompare(Expression left, Expression right, Esprima.Ast.BinaryOperator op) {
            switch(op)
            {
                case Esprima.Ast.BinaryOperator.Less:
                    return Expression.LessThan(left, right);
            }
            throw new NotImplementedException();
        }

        private static Expression StringCompare(Expression left, Expression right, Esprima.Ast.BinaryOperator op)
        {
            switch (op)
            {
                case Esprima.Ast.BinaryOperator.Less:
                    return 
                        Expression.LessThan(ExpHelper.String.Compare(left, right), Expression.Constant((int)0));
            }
            throw new NotImplementedException();
        }

        static Expression CompareStrictlyEqual(Expression left, Expression right)
        {
            var leftBoolean = Expression.Variable(typeof(JSBoolean));
            var rightBoolean = Expression.Variable(typeof(JSBoolean));
            
            var leftNumber = Expression.Variable(typeof(JSNumber));
            var rightNumber = Expression.Variable(typeof(JSNumber));
            
            var leftString = Expression.Variable(typeof(JSString));
            var rightString = Expression.Variable(typeof(JSString));

            var nullBoolean = Expression.Constant(null, typeof(JSBoolean));
            var nullNumber = Expression.Constant(null, typeof(JSNumber));
            var nullString = Expression.Constant(null, typeof(JSString));

            var compareBoolean =
                Expression.Block(new ParameterExpression[] { rightBoolean },
                Expression.Assign(rightBoolean, Expression.TypeAs(right, typeof(JSBoolean))),
                Expression.Condition(
                    Expression.NotEqual(nullBoolean, rightBoolean),
                    Expression.Condition(
                        Expression.Equal( ExpHelper.JSBoolean.Value(leftBoolean), ExpHelper.JSBoolean.Value(rightBoolean) ),
                        ExpHelper.JSContext.True,
                        ExpHelper.JSContext.False),
                    ExpHelper.JSContext.False
                ));

            var compareNumber =
                Expression.Block(new ParameterExpression[] { rightNumber },
                    Expression.Assign(rightNumber, Expression.TypeAs(right, typeof(JSNumber))),
                    Expression.Condition(
                        Expression.NotEqual(nullNumber,rightNumber),
                        Expression.Condition(
                            Expression.Equal(ExpHelper.JSNumber.Value(leftNumber), ExpHelper.JSNumber.Value(rightNumber)),
                            ExpHelper.JSContext.True,
                            ExpHelper.JSContext.False),
                        ExpHelper.JSContext.False
                ));

            var compareString =
                Expression.Block(new ParameterExpression[] { rightString },
                    Expression.Assign(rightString, Expression.TypeAs(right, typeof(JSString))),
                    Expression.Condition(
                        Expression.NotEqual(nullString, rightString),
                        Expression.Condition(
                            ExpHelper.String.Equals(ExpHelper.JSString.Value(leftString), ExpHelper.JSString.Value(rightString)),
                            ExpHelper.JSContext.True,
                            ExpHelper.JSContext.False),
                        ExpHelper.JSContext.False
                ));

            return Expression.Condition(
                        Expression.And(Expression.TypeIs(left, typeof(JSNull)), Expression.TypeIs(right, typeof(JSNull))),
                        ExpHelper.JSContext.True,

                        Expression.Condition(
                            Expression.And(Expression.TypeIs(left, typeof(JSUndefined)), Expression.TypeIs(right, typeof(JSUndefined))),
                            ExpHelper.JSContext.True,

                            Expression.Block(
                                new ParameterExpression[] { leftNumber },
                                Expression.Assign(leftNumber, Expression.TypeAs(left, typeof(JSNumber))),
                                Expression.Condition(
                                    Expression.NotEqual(nullNumber, leftNumber),
                                    compareNumber,
                                    Expression.Block(
                                        new ParameterExpression[] { leftString },
                                        Expression.Assign(leftString, Expression.TypeAs(left, typeof(JSString))),
                                        Expression.Condition(
                                            Expression.NotEqual(nullString, leftString),
                                            compareString,
                                            Expression.Block(
                                                new ParameterExpression[] { leftBoolean },
                                                Expression.Assign(leftBoolean, Expression.TypeAs(left, typeof(JSBoolean))),
                                                Expression.Condition(
                                                        Expression.NotEqual(nullBoolean, leftBoolean),
                                                        compareBoolean,
                                                        ExpHelper.JSContext.False
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )

                        )
                    )
                ;
        }
    }
}
