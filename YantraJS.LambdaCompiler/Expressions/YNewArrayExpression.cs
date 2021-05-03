#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YNewArrayExpression: YExpression
    {
        public readonly YExpression[]? Elements;
        public readonly Type ElementType;

        public YNewArrayExpression(Type type, YExpression[] elements)
            : base( YExpressionType.NewArray, type.MakeArrayType())
        {
            this.ElementType = type;
            this.Elements = elements;
        }

        public override void Print(IndentedTextWriter writer)
        {
            if (Elements == null || Elements.Length == 0){
                writer.WriteLine($"new {ElementType.GetFriendlyName()} [] {{}}");
                return;
            }

            writer.WriteLine($"new {ElementType.GetFriendlyName()} [] {{");
            writer.Indent++;
            foreach (var a in Elements)
            {
                a.Print(writer);
                writer.WriteLine(',');
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}