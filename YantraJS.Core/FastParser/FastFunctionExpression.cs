namespace YantraJS.Core.FastParser
{
    public class FastFunctionExpression : FastExpression, IVariableScope
    {

        public readonly SparseList<FastNode> Parameters = new SparseList<FastNode>();
        public bool IsGenerator { get; private set; }

        public readonly bool IsAsync;

        public readonly bool IsArrow;

        public FastFunctionExpression(FastNode parent, FastTokenStream stream, bool isAsync = false) 
            : base(parent, FastNodeType.FunctionExpression, stream)
        {
            this.IsAsync = isAsync;
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
            var current = stream.Current;
            if (current.IsKeyword)
            {
                stream.Consume();
            }

            // check if it is generator
            if(stream.CheckAndConsume(TokenTypes.Multiply))
            {
                this.IsGenerator = true;
            }

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
