namespace YantraJS.Core.FastParser
{
    public class FastExpressionStatement : FastStatement
    {
        private readonly FastKeywords hint;
        public FastExpression Expresion;

        public FastExpressionStatement(
            FastNode parent, 
            FastTokenStream stream, FastKeywords hint = FastKeywords.none) : base(parent, FastNodeType.ExpressionStatement, stream)
        {
            this.hint = hint;
        }

        internal override void Read(FastTokenStream stream)
        {
            this.Expresion = FastExpression.Read(this, stream, hint);
        }
    }
}
