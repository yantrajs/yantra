using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{

    public class FastBlock: FastStatement, IVariableScope
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

        public FastVariable CreateVariable(string name, bool scoped)
        {
            throw new System.NotImplementedException();
        }

        public FastVariable GetVariable(string alias)
        {
            throw new System.NotImplementedException();
        }
    }
}
