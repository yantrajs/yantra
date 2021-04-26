#nullable enable
using System.Reflection;

namespace YantraJS.Expressions
{
    public class YPropertyExpression : YExpression
    {
        public readonly YExpression Target;
        public readonly PropertyInfo PropertyInfo;
        public readonly MethodInfo? GetMethod;
        public readonly MethodInfo? SetMethod;

        public YPropertyExpression(YExpression target, PropertyInfo property)
            : base(YExpressionType.Property, property.PropertyType)
        {
            this.Target = target;
            this.PropertyInfo = property;
            if (property.CanRead)
            {
                GetMethod = property.GetMethod;
            }
            if(property.CanWrite)
            {
                SetMethod = property.SetMethod;
            }
        }
    }

}