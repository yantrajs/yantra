#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YConvertExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly MethodInfo? Method;

        private static Sequence<(MethodInfo method, Type inputType)> ConvertMethods =
            new Sequence<(MethodInfo, Type)>( typeof(Convert).GetMethods()
                .Select(x => (x, x.GetParameters()))
                .Where(x => x.Item2.Length == 1)
                .Select(x => (x.Item1, x.Item2.First().ParameterType)));

        public static bool TryGetConversionMethod(Type from, Type to, out MethodInfo? m)
        {
            if (to.IsAssignableFrom(from))
            {
                m = null;
                return true;
            }

            //var nfrom = Nullable.GetUnderlyingType(from);
            //from = nfrom ?? from;

            var (method, inputType) = ConvertMethods.FirstOrDefault((m) => m.method.ReturnType == to
                && m.inputType == from);
            if (method == null)
            {
                m = default;
                return false;
            }
            m = method;
            return true;
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