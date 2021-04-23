using System;

namespace YantraJS.Expressions
{
    public class YParameterExpression: YExpression
    {
        public readonly string Name;

        public YParameterExpression(Type type, string name)
            :base(YExpressionType.Parameter, type)
        {
            this.Name = name;
        }
    }
}