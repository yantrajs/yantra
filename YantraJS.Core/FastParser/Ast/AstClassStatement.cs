namespace YantraJS.Core.FastParser
{
    internal class AstClassStatement : AstStatement
    {
        public readonly AstIdentifier Identifier;
        public readonly AstExpression Base;
        public readonly AstClassProperty[] Members;

        public AstClassStatement(FastToken token, FastToken previousToken, AstIdentifier identifier, AstExpression @base, AstClassProperty[] astClassProperties)
            : base(token,  FastNodeType.ClassStatement, previousToken)
        {
            this.Identifier = identifier;
            this.Base = @base;
            this.Members = astClassProperties;
        }
    }
}