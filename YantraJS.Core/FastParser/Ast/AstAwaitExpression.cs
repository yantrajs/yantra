namespace YantraJS.Core.FastParser
{
    public class AstAwaitExpression : AstExpression
    {
        public readonly AstExpression Argument;

        public AstAwaitExpression(FastToken token, FastToken previousToken, AstExpression target)
            : base(token, FastNodeType.AwaitExpression, previousToken)
        {
            this.Argument = target;
        }
    }
}