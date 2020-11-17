using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.ExpHelper;

namespace YantraJS.Core.LinqExpressions.Logical
{
    //internal static class LogicalBoolean
    //{
    //    internal static Expression Build(
    //        bool booleanValue, 
    //        Esprima.Ast.BinaryOperator @operator, 
    //        Expression expression)
    //    {
    //        switch (@operator)
    //        {
    //            case Esprima.Ast.BinaryOperator.Equal:
    //                if (booleanValue)
    //                {
    //                    return JSBooleanBuilder.NewFromCLRBoolean(JSValueBuilder.BooleanValue(expression));
    //                }
    //                return JSBooleanBuilder.NewFromCLRBoolean(Expression.Not(JSValueBuilder.BooleanValue(expression)));
    //            case Esprima.Ast.BinaryOperator.NotEqual:
    //                if (!booleanValue)
    //                {
    //                    return JSBooleanBuilder.NewFromCLRBoolean(JSValueBuilder.BooleanValue(expression));
    //                }
    //                return JSBooleanBuilder.NewFromCLRBoolean(Expression.Not(JSValueBuilder.BooleanValue(expression)));
    //            case Esprima.Ast.BinaryOperator.StrictlyEqual:
    //                Expression.Coalesce(Expression.TypeAs(expression, typeof(JSBoolean)))
    //                break;
    //            case Esprima.Ast.BinaryOperator.StricltyNotEqual:
    //                break;
    //        }
    //    }
    //}

    //internal static class LogicalNumeric
    //{
    //    internal static Expression Build(double numericValue, Esprima.Ast.BinaryOperator @operator, Expression expression)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //internal static class LogicalString
    //{
    //    internal static Expression Build(string stringValue, Esprima.Ast.BinaryOperator @operator, Expression expression)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //internal static class LogicalNull
    //{
    //    internal static Expression Build(Esprima.Ast.BinaryOperator @operator, Expression expression)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
