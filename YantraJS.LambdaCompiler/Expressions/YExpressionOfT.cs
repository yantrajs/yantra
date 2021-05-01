using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public class YExpression<T> : YLambdaExpression
    {
        public YExpression(string name, YExpression body, YParameterExpression[] parameters, Type returnType)
            : base(typeof(T), name, body, parameters, returnType)
        {
        }
    }
}
