#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace YantraJS.Expressions
{
    public class YDelegateExpression: YExpression
    {
        public readonly MethodInfo Method;

        public YDelegateExpression(MethodInfo method, Type? type = null)
            : base(YExpressionType.Delegate, type ?? GetSignature(method))
        {
            this.Method = method;
        }

        private static Type GetSignature(MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write($"delegate({Method.Name})");
        }
    }
}