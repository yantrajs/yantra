namespace YantraJS.Core.FastParser
{
    public class FastVariableDeclaration : FastStatement
    {

        public readonly struct FastDeclarator
        {
            public readonly FastToken Identifier;
            public readonly FastExpression Init;
            public FastDeclarator(FastToken token, FastExpression init = null)
            {
                Identifier = token;
                this.Init = init;
            }
        }

        public readonly bool isLet;
        public readonly bool isConst;

        public readonly SparseList<FastDeclarator> Declarators = new SparseList<FastDeclarator>();

        public FastVariableDeclaration(FastBlock parent, FastTokenStream stream, bool isLet = false, bool isConst = false)
            : base(parent, FastNodeType.VariableDeclaration)
        {
            this.isLet = isLet;
            this.isConst = isConst;
            Read(stream);
        }

        internal override void Read(FastTokenStream stream)
        {
            while (true)
            {
                var id = stream.Expect(TokenTypes.Identifier);
                if (stream.CheckAndConsume(TokenTypes.Assign))
                {
                    var init = FastExpression.Read(this, stream);
                    Declarators.Add(new FastDeclarator(id, init));
                    if(stream.CheckAndConsume(TokenTypes.Comma))
                    {
                        continue;
                    }
                }
                Declarators.Add(new FastDeclarator(id));
                if (stream.CheckAndConsume(TokenTypes.Comma))
                {
                    continue;
                }
                break;
            }
        }
    }
}
