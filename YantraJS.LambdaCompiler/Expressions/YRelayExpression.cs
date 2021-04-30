using System;
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YRelayExpression: YExpression
    {
        public readonly YExpression Closure;
        public readonly YLambdaExpression InnerLambda;

        public YRelayExpression(YExpression box, YLambdaExpression inner, Type originalType)
            : base(YExpressionType.Relay, originalType)
        {
            this.Closure = box;
            this.InnerLambda = inner;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("relay(");
            Closure.Print(writer);
            writer.Write(", ");
            InnerLambda.Print(writer);
            writer.Write(")");
        }
    }
}