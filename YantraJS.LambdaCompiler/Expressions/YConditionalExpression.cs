#nullable enable
using System;

namespace YantraJS.Expressions
{
    internal class YConditionalExpression : YExpression
    {
        public readonly YExpression test;
        public readonly YExpression @true;
        public readonly YExpression @false;

        public YConditionalExpression(
            YExpression test, 
            YExpression @true, 
            YExpression @false,
            Type? type)
            :base(YExpressionType.Conditional, type ?? @true.Type)
        {
            this.test = test;
            this.@true = @true;
            this.@false = @false;
        }
    }
}