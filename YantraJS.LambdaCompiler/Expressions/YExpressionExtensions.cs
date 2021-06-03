using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public static class YExpressionExtensions
    {

        public static void PrintCSV<T>(this IndentedTextWriter writer, IList<T> items)
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
