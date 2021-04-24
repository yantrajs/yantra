using System.Reflection;

namespace YantraJS.Expressions
{
    public class YFieldExpression: YExpression
    {
        public readonly YExpression Target;
        public readonly FieldInfo FieldInfo;

        public YFieldExpression(YExpression target, FieldInfo field)
            : base(YExpressionType.Field, field.FieldType)
        {
            this.Target = target;
            this.FieldInfo = field;
        }
    }

}