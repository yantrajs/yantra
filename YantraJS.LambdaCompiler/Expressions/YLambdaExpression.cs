#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YLambdaExpression: YExpression
    {
        public readonly string Name;
        public readonly YExpression Body;
        public readonly YParameterExpression[] Parameters;
        public readonly Type ReturnType;

        public YLambdaExpression(string name, 
            YExpression body, 
            IList<YParameterExpression>? parameters,
            Type? returnType = null)
            : base(YExpressionType.Lambda, body.Type)
        {
            this.Name = name;
            this.Body = body;
            this.ReturnType = returnType ?? body.Type;
            if (parameters != null)
                this.Parameters = parameters.ToArray();
            else
                this.Parameters = new YParameterExpression[] { };
        }
    }
}