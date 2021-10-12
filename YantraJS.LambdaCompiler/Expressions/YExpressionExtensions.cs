using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Expressions
{
    public static class YExpressionExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Is<T>(this YExpression exp, YExpressionType type, out T value)
        {
            if(exp.NodeType == type && exp is T texp)
            {
                value = texp;
                return true;
            }
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConstant(this YExpression exp, out YConstantExpression c)
            => Is<YConstantExpression>(exp, YExpressionType.Constant, out c);

        public static void PrintCSV<T>(this IndentedTextWriter writer, IEnumerable<T> items)
            where T: YExpression
        {
            bool first = true;
            foreach(var item in items)
            {
                if (!first)
                    writer.Write(", ");
                first = false;
                item.Print(writer);
            }
        }

    }
}
