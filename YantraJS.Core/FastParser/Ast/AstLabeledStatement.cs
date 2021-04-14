namespace YantraJS.Core.FastParser
{
    public class AstLabeledStatement : AstStatement
    {
        public readonly FastToken Label;
        public readonly AstStatement Body;

        public AstLabeledStatement(FastToken id, AstStatement statement)
            : base (id, FastNodeType.LabeledStatement, statement.End)
        {
            this.Label = id;
            this.Body = statement;
        }

        public override string ToString()
        {
            return $"{Label}: {Body}";
        }
    }
}