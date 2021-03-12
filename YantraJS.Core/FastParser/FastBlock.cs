using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{

    public class FastBlock: FastStatement
    {
        public readonly SparseList<FastStatement> Body = new SparseList<FastStatement>();

        protected FastBlock(FastNode parent, FastNodeType type, FastTokenStream stream) : base(parent, type, stream) { }

        public FastBlock(FastNode parent, FastTokenStream stream): base(parent, FastNodeType.Block, stream)
        {

        }

        internal override void Read(FastTokenStream stream)
        {
            while (ParseStatement(this, stream, out FastStatement statement))
                Body.Add(statement);
        }
    }
}
