#nullable enable
using System.Reflection;

namespace YantraJS.Expressions
{
    public class YIndexExpression: YExpression
    {
        public readonly YExpression Target;
        public new readonly PropertyInfo Property;
        public readonly YExpression[] Arguments;

        public YIndexExpression(YExpression target, PropertyInfo propertyInfo, YExpression[] args)
            : base(YExpressionType.Index, propertyInfo.PropertyType)
        {
            this.Target = target;
            this.Property = propertyInfo;
            this.Arguments = args;
        }
    }
}