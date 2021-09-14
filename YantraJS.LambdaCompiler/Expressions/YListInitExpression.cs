using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace YantraJS.Expressions
{
    public class YElementInit
    {
        public readonly MethodInfo AddMethod;
        public readonly YExpression[] Arguments;

        public YElementInit(MethodInfo addMethod, YExpression[] arguments)
        {
            this.AddMethod = addMethod;
            this.Arguments = arguments;
        }
    }

    public class YListInitExpression : YExpression
    {
        public readonly YNewExpression NewExpression;
        public readonly YElementInit[] Members;

        public YListInitExpression(
            YNewExpression newExpression,
            YElementInit[] parameters) : base(YExpressionType.ListInit, newExpression.Type)
        {
            this.NewExpression = newExpression;
            this.Members = parameters;
        }

        public override void Print(IndentedTextWriter writer)
        {
            NewExpression.Print(writer);
            writer.Write(" {");
            writer.Indent++;
            foreach(var e in Members)
            {
                writer.Write("{");
                foreach(var p in e.Arguments)
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
