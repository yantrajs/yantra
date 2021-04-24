using System.Reflection;

namespace YantraJS.Expressions
{
    public class YPropertyExpression : YExpression
    {
        public readonly YExpression Target;
        public readonly PropertyInfo PropertyInfo;

        public YPropertyExpression(YExpression target, PropertyInfo field)
            : base(YExpressionType.Property, field.PropertyType)
        {
            this.Target = target;
            this.PropertyInfo = field;
        }
    }

}