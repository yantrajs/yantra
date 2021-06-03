using System;
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YConstantExpression : YExpression
    {
        public readonly object Value;

        public YConstantExpression(object value, Type type)
            : base(YExpressionType.Constant, type)
        {
            this.Value = value;
        }

        public override void Print(IndentedTextWriter writer)
        {
            if (Value == null)
            {
                writer.Write("null");
                return;
            }
            if(Type == typeof(string))
            {
                writer.Write($"\"{Escape(Value.ToString())}\"");
                return;
            }
            writer.Write(Value);
        }

        private string Escape(string text)
        {
            return text
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");
        }
    }
}