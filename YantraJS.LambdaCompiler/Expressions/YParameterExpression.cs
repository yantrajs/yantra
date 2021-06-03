#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Threading;

namespace YantraJS.Expressions
{
    public class YParameterExpression: YExpression
    {
        public readonly string Name;

        private static int id = 0;

        public YParameterExpression(Type type, string? name)
            :base(YExpressionType.Parameter, type)
        {
            if(name == null)
            {
                name = $"{type.Name}_{Interlocked.Increment(ref id)}";
            }
            this.Name = name;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write(Name);
        }
    }
}