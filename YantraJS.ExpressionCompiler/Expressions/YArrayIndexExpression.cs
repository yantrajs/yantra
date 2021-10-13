using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YArrayIndexExpression: YExpression
    {
        public readonly YExpression Target;
        public new readonly YExpression Index;

        public YArrayIndexExpression(YExpression target, YExpression index)
            : base(YExpressionType.ArrayIndex, target.Type.GetElementType())
        {
            this.Target = target;
            this.Index = index;
        }

        public override void Print(IndentedTextWriter writer)
        {
            Target.Print(writer);
            writer.Write("[");
            Index.Print(writer);
            writer.Write("]");
        }
    }
}