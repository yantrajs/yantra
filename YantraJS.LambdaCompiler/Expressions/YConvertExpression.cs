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

        public static bool TryGetConversionMethod(Type from, Type to, out MethodInfo? m)
        {
            if (to.IsAssignableFrom(from))
            {
                m = null;
                return true;
            }

            //var nfrom = Nullable.GetUnderlyingType(from);
            //from = nfrom ?? from;

            var c = typeof(Convert).GetMethods();
            m = c.FirstOrDefault(m => m.ReturnType == to
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == from);
            if (m == null)
                return false;

            return true;
        }

        public YConvertExpression(YExpression exp, Type type, MethodInfo? method)
            : base(YExpressionType.Convert, type)
        {
            this.Target = exp;
            if (method == null)
                throw new ArgumentNullException($"Method {method} cannot be null to convert from {exp.Type} to {type}");
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