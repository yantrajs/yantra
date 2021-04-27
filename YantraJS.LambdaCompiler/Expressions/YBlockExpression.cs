#nullable enable
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YBlockExpression: YExpression
    {
        public readonly YParameterExpression[] Variables;
        public readonly YExpression[] Expressions;

        public YBlockExpression(IEnumerable<YParameterExpression>? variables, IList<YExpression> expressions)
            :base(YExpressionType.Block, expressions.Last().Type)
        {
            this.Variables = variables?.ToArray() ?? (new YParameterExpression[] { });
            this.Expressions = expressions.ToArray();
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine("{");
            writer.Indent++;
            foreach (var v in Variables)
            {
                writer.WriteLine($"{v.Type.FullName} {v.Name};");
            }
            foreach (var exp in Expressions)
            {
                exp.Print(writer);
                writer.WriteLine(";");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}