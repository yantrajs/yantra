using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{

    public class FastBlock: FastStatement
    {
        public readonly SparseList<FastStatement> Body = new SparseList<FastStatement>();

        protected FastBlock(FastNode parent, FastNodeType type) : base(parent, type) { }

        public FastBlock(FastNode parent): base(parent, FastNodeType.Block)
        {

        }

        internal override void Read(FastTokenStream stream)
        {
            do
            {
                var token = stream.Current;
                if(stream.Keywords.IsKeyword(in token.Span, out var keyword))
                {
                    stream.Consume();
                    switch (keyword)
                    {

                        /**
                         * Variable Declarations
                         */
                        case FastKeywords.let:
                            Body.Add(new FastVariableDeclaration(this, stream, isLet: true));
                            continue;
                        case FastKeywords.var:
                            Body.Add(new FastVariableDeclaration(this, stream));
                            continue;
                        case FastKeywords.@const:
                            Body.Add(new FastVariableDeclaration(this, stream, isConst: true));
                            continue;


                    }
                }
            } while (true);
        }
    }
}
