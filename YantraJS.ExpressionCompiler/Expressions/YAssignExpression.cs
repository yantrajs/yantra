#nullable enable
using System;
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YAssignExpression : YExpression
    {
        public readonly YExpression Left;
        public readonly YExpression Right;

        public YAssignExpression(YExpression left, YExpression right, Type? type)
            : base(YExpressionType.Assign, type ?? left.Type)
        {
            this.Left = left;
            this.Right = right;
        }

        public override void Print(IndentedTextWriter writer)
        {
            Left.Print(writer);
            writer.Write(" = ");
            Right.Print(writer);
        }
    }
}