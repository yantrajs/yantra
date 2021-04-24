#nullable enable
using System;

namespace YantraJS.Expressions
{
    public class YConditionalExpression : YExpression
    {
        public readonly YExpression test;
        public readonly YExpression @true;
        public readonly YExpression @false;

        public YConditionalExpression(
            YExpression test, 
            YExpression @true, 
            YExpression @false,
            Type? type = null)
            :base(YExpressionType.Conditional, type ?? @true.Type)
        {
            this.test = test;
            this.@true = @true;
            this.@false = @false;
        }
    }
}