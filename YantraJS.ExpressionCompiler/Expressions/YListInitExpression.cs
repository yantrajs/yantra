using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Expressions
{


    public class YListInitExpression : YExpression
    {
        public readonly YNewExpression NewExpression;
        public readonly IFastEnumerable<YElementInit> Members;

        public YListInitExpression(
            YNewExpression newExpression,
            IFastEnumerable<YElementInit> parameters) : base(YExpressionType.ListInit, newExpression.Type)
        {
            this.NewExpression = newExpression;
            this.Members = parameters;
        }

        public override void Print(IndentedTextWriter writer)
        {
            NewExpression.Print(writer);
            writer.Write(" {");
            writer.Indent++;
            var en = Members.GetFastEnumerator();
            while(en.MoveNext(out var e))
            {
                writer.Write("{");
                var enp = e.Arguments.GetFastEnumerator();
                while(enp.MoveNext(out var p))
                {
                    p.Print(writer);
                    writer.Write(",");
                }
                writer.WriteLine("},");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
