#nullable enable
using System;
using System.CodeDom.Compiler;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YInvokeExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly IFastEnumerable<YExpression> Arguments;

        public YInvokeExpression(YExpression target, IFastEnumerable<YExpression> args, Type type)
            : base(YExpressionType.Invoke, type)
        {
            this.Target = target;
            this.Arguments = args;
        }

        public override void Print(IndentedTextWriter writer)
        {
            Target.Print(writer);
            writer.Write(".Invoke(");
            writer.PrintCSV(Arguments);
            writer.Write(")");
        }
    }
}