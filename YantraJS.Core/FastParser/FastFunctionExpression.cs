namespace YantraJS.Core.FastParser
{
    public class FastAssignmentPattern : FastExpression
    {
        public readonly FastNode Left;
        public readonly FastNode Right;

        public FastAssignmentPattern(FastNode parent, FastTokenStream stream) 
            : base(parent, FastNodeType.AssignmentPattern, stream)
        {
        }

        internal override void Read(FastTokenStream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    public class FastFunctionExpression : FastExpression, IVariableScope
    {

        public readonly SparseList<FastNode> Parameters = new SparseList<FastNode>();

        public FastFunctionExpression(FastNode parent, FastTokenStream stream) 
            : base(parent, FastNodeType.FunctionExpression, stream)
        {
        }

        public FastToken Identifier { get; private set; }

        public FastVariable CreateVariable(string name, bool scoped)
        {
            throw new System.NotImplementedException();
        }

        public FastVariable GetVariable(string alias)
        {
            throw new System.NotImplementedException();
        }

        internal override void Read(FastTokenStream stream)
        {
            // consume function keyword...
            stream.Consume();
            // name of the function...
            if(stream.CheckAndConsume(TokenTypes.Identifier, out var token))
            {
                this.Identifier = token;
            }

            stream.Expect(TokenTypes.BracketStart);

            var destructer = false;
            // parse parameters...
            do {

                if(stream.CheckAndConsume( TokenTypes.CurlyBracketStart))
                {
                    destructer = true;
                }

                var spread = false;
                if (stream.CheckAndConsume(TokenTypes.TripleDots))
                {
                    spread = true;
                }
                var identifier = stream.Expect(TokenTypes.Identifier);
            }
            while (true);
        }
    }
}
