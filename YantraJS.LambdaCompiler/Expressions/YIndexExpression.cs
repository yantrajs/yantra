#nullable enable
using System.CodeDom.Compiler;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YIndexExpression: YExpression
    {
        public readonly YExpression Target;
        public new readonly PropertyInfo Property;
        public readonly IFastEnumerable<YExpression> Arguments;
        public readonly MethodInfo? SetMethod;
        public readonly MethodInfo? GetMethod;

        public YIndexExpression(YExpression target, PropertyInfo propertyInfo, IFastEnumerable<YExpression> args)
            : base(YExpressionType.Index, propertyInfo.PropertyType)
        {
            this.Target = target;
            this.Property = propertyInfo;
            this.Arguments = args;
            GetMethod = propertyInfo.GetMethod;
            SetMethod = propertyInfo.SetMethod;
        }

        public override void Print(IndentedTextWriter writer)
        {
            Target?.Print(writer);
            writer.Write('[');
            writer.PrintCSV(Arguments);
            writer.Write(']');
        }
    }
}