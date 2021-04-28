using System.CodeDom.Compiler;
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

        public override void Print(IndentedTextWriter writer)
        {
            if(Target==null)
            {
                writer.Write($"{FieldInfo.DeclaringType.GetFriendlyName()}.{FieldInfo.Name}");
                return;
            }
            Target.Print(writer);
            writer.Write('.');
            writer.Write(FieldInfo.Name);
        }
    }

}