#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YNewArrayExpression: YExpression
    {
        public readonly IFastEnumerable<YExpression>? Elements;
        public readonly Type ElementType;

        public YNewArrayExpression(Type type, IFastEnumerable<YExpression> elements)
            : base( YExpressionType.NewArray, type.MakeArrayType())
        {
            this.ElementType = type;
            this.Elements = elements;
        }

        public override void Print(IndentedTextWriter writer)
        {
            if (Elements == null || Elements.Count == 0){
                writer.WriteLine($"new {ElementType.GetFriendlyName()} [] {{}}");
                return;
            }

            writer.WriteLine($"new {ElementType.GetFriendlyName()} [] {{");
            writer.Indent++;
            var en = Elements.GetFastEnumerator();
            while(en.MoveNext(out var a))
            {
                a.Print(writer);
                writer.WriteLine(',');
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}