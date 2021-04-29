#nullable enable
using System.CodeDom.Compiler;
using System.Reflection;

namespace YantraJS.Expressions
{
    public class YPropertyExpression : YExpression
    {
        public readonly YExpression Target;
        public readonly PropertyInfo PropertyInfo;
        public readonly MethodInfo? GetMethod;
        public readonly MethodInfo? SetMethod;
        public readonly bool IsStatic;

        public YPropertyExpression(YExpression target, PropertyInfo property)
            : base(YExpressionType.Property, property.PropertyType)
        {
            this.Target = target;
            this.PropertyInfo = property;

            if (property.CanRead)
            {
                GetMethod = property.GetMethod;
                this.IsStatic = GetMethod.IsStatic;
            }
            if(property.CanWrite)
            {
                SetMethod = property.SetMethod;
                this.IsStatic = SetMethod.IsStatic;
            }
        }

        public override void Print(IndentedTextWriter writer)
        {
            if (Target == null)
            {
                writer.Write($"{PropertyInfo.DeclaringType.GetFriendlyName()}.{PropertyInfo.Name}");
                return;
            }
            Target.Print(writer);
            writer.Write('.');
            writer.Write(PropertyInfo.Name);
        }
    }

}