using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{
    public class FastBlock: FastStatement
    {
        public List<FastStatement> Statements;

        public FastBlock(): base(FastNodeType.Block)
        {

        }
    }
}
