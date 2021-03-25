namespace YantraJS.Core.FastParser
{
    public class AstObjectLiteral : AstExpression
    {
        public readonly ObjectProperty[] Members;

        public AstObjectLiteral(FastToken token, FastToken previousToken, ObjectProperty[] objectProperties)
            : base (token, FastNodeType.ObjectLiteral, previousToken)
        {
            this.Members = objectProperties;
        }
    }
}