using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public class YExpression<T> : YLambdaExpression
    {
        public YExpression(in FunctionName name, YExpression body, YParameterExpression @this, YParameterExpression[] parameters, Type returnType)
            : base(typeof(T), in name, body, @this, parameters, returnType)
        {
        }

        internal YExpression<T1> WithThis<T1>(Type type)
        {
            if (This != null)
                throw new InvalidOperationException();
            return new YExpression<T1>(in Name, Body, YExpression.Parameter(type), Parameters, ReturnType);
        }
    }
}
