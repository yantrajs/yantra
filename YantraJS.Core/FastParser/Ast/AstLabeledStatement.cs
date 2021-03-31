namespace YantraJS.Core.FastParser
{
    public class AstLabeledStatement : AstStatement
    {
        public readonly FastToken Identifier;
        public readonly AstStatement Statement;

        public AstLabeledStatement(FastToken id, AstStatement statement)
            : base (id, FastNodeType.LabeledStatement, statement.End)
        {
            this.Identifier = id;
            this.Statement = statement;
        }
    }
}