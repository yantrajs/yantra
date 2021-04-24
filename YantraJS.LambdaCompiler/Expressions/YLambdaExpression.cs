using System.Collections.Generic;

namespace YantraJS.Expressions
{
    public class YLambdaExpression: YExpression
    {
        public readonly string Name;
        public readonly YExpression Body;
        public readonly IList<YParameterExpression> Parameters;

        public YLambdaExpression(string name, 
            YExpression body, 
            IList<YParameterExpression> parameters)
            : base(YExpressionType.Lambda, body.Type)
        {
            this.Name = name;
            this.Body = body;
            this.Parameters = parameters;
        }
    }
}