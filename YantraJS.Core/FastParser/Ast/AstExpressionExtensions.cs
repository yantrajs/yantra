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


    }
}
