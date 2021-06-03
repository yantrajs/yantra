#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstConditionalExpression : AstExpression
    {
        public readonly AstExpression Test;
        public readonly AstExpression True;
        public readonly AstExpression False;

        public AstConditionalExpression(AstExpression previous, AstExpression @true, AstExpression @false)
            : base(previous.Start, FastNodeType.ConditionalExpression, @false.End)
        {
            this.Test = previous;
            this.True = @true;
            this.False = @false;
        }

        public override string ToString()
        {
            return $"{Test} ? {True} : {False}";
        }
    }
}