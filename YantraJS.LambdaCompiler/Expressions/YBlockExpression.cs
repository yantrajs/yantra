#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YBlockExpression: YExpression
    {
        public readonly YParameterExpression[] Variables;
        public readonly YExpression[] Expressions;

        public YBlockExpression(IEnumerable<YParameterExpression>? variables, 
            YExpression[] expressions)
            :base(YExpressionType.Block, expressions.Last().Type)
        {
            this.Variables = variables?.ToArray() ?? (new YParameterExpression[] { });
            if (this.Variables.Any(v => v == null))
                throw new ArgumentNullException();
            this.Expressions = expressions;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine("{");
            writer.Indent++;
            foreach (var v in Variables)
            {
                writer.WriteLine($"{v.Type.GetFriendlyName()} {v.Name};");
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