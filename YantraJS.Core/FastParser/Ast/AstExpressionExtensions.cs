using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.FastParser;

namespace YantraJS.Core
{
    public static class AstExpressionExtensions
    {


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Is<T>(this AstNode exp, FastNodeType type, out T value)
        {
            if (exp.Type == type && exp is T texp)
            {
                value = texp;
                return true;
            }
            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFunction(this AstNode exp, out AstFunctionExpression value)
            => Is(exp, FastNodeType.FunctionExpression, out value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsExpressionStatement(this AstNode exp, out AstExpressionStatement value)
            => Is(exp, FastNodeType.ExpressionStatement, out value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSpreadElement(this AstNode node, out AstSpreadElement value)
            => Is(node, FastNodeType.SpreadElement, out value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnaryExpression(this AstNode exp, out AstUnaryExpression unary)
            => Is(exp, FastNodeType.UnaryExpression, out unary);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBinaryExpression(this AstNode exp, out AstBinaryExpression binary)
            => Is(exp, FastNodeType.BinaryExpression, out binary);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStringLiteral(this AstNode exp, out string value)
        {
            if (Is<AstLiteral>(exp, FastNodeType.Literal, out var literall))
            {
                if (literall.TokenType == TokenTypes.String)
                {
                    value = literall.StringValue;
                    return true;
                }
            }
            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUIntLiteral(this AstNode exp, out uint value)
        {
            if (Is<AstLiteral>(exp, FastNodeType.Literal, out var literall))
            {
                if (literall.TokenType == TokenTypes.Number)
                {
                    var n = literall.NumericValue;
                    value = 0;
                    if (n == 0)
                    {
                        return true;
                    }
                    if (n > 0 && n % 1 == 0)
                    {
                        value = (uint)n;
                        return true;
                    }
                    return false;
                }
            }
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumberLiteral(this AstNode exp, out double value)
        {
            if (Is<AstLiteral>(exp, FastNodeType.Literal, out var literall))
            {
                if (literall.TokenType == TokenTypes.Number)
                {
                    value = literall.NumericValue;
                    return true;
                }
            }
            value = default;
            return false;
        }
    }
}
