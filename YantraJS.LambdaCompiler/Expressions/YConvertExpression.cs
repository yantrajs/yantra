#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;

namespace YantraJS.Expressions
{
    public class YConvertExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly MethodInfo? Method;

        public static MethodInfo? GetConversionMethod(Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
                return null;

            //var nfrom = Nullable.GetUnderlyingType(from);
            //from = nfrom ?? from;

            var c = typeof(Convert).GetMethods();
            var m = c.FirstOrDefault(m => m.ReturnType == to
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == from);
            if (m == null)
                throw new InvalidOperationException($"No conversion method found from {from.FullName} to {to.FullName}");

            return m;
        }

        public YConvertExpression(YExpression exp, Type type, MethodInfo? method)
            : base(YExpressionType.Convert, type)
        {
            this.Target = exp;
            Method = method;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("convert(");
            Target.Print(writer);
            writer.Write(")");
        }
    }
}