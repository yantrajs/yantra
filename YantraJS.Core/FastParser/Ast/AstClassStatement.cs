namespace YantraJS.Core.FastParser
{
    public class AstClassExpression : AstExpression
    {
        public readonly AstIdentifier Identifier;
        public readonly AstExpression Base;
        public readonly ArraySpan<AstClassProperty> Members;

        public AstClassExpression(FastToken token, FastToken previousToken, AstIdentifier identifier, AstExpression @base, 
            ArraySpan<AstClassProperty> astClassProperties)
            : base(token,  FastNodeType.ClassStatement, previousToken)
        {
            this.Identifier = identifier;
            this.Base = @base;
            this.Members = astClassProperties;
        }
    }
}