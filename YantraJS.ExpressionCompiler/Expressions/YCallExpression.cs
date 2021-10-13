#nullable enable
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YCallExpression: YExpression
    {
        public readonly YExpression? Target;
        public readonly MethodInfo Method;
        public readonly IFastEnumerable<YExpression> Arguments;

        public YCallExpression(YExpression? target, MethodInfo method, IFastEnumerable<YExpression> args)
            : base(YExpressionType.Call, method.ReturnType)
        {
            this.Target = target;
            this.Method = method;
            this.Arguments = args;
        }

        public override void Print(IndentedTextWriter writer)
        {
            if (Target == null)
            {
                // static method...
                writer.Write($"{Method.DeclaringType.GetFriendlyName()}.{Method.Name}(");
            }
            else
            {
                Target.Print(writer);
                writer.Write($".{Method.Name}(");
            }
            writer.PrintCSV(Arguments);
            writer.Write(')');
        }
    }
}