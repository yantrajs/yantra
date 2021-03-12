namespace YantraJS.Core.FastParser
{
    public class FastParameter
    {

    }

    public class FastFunctionExpression : FastExpression
    {

        public readonly SparseList<FastParameter> Parameters = new SparseList<FastParameter>();

        public FastFunctionExpression(FastNode parent, FastTokenStream stream) 
            : base(parent, FastNodeType.FunctionExpression, stream)
        {
        }

        public FastToken Identifier { get; private set; }

        internal override void Read(FastTokenStream stream)
        {

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
