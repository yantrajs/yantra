using System.Linq;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public delegate bool ParseToken(FastScanner scanner, out FastToken token);

    public class FastProgram: FastStatement {

        public readonly SparseList<FastStatement> Body = new SparseList<FastStatement>();
        public FastProgram(): base(FastNodeType.Program)
        {

        }

        

    }
}
