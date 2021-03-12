using System.Linq;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public delegate bool ParseToken(FastScanner scanner, out FastToken token);

    public class FastProgram: FastBlock {

        public FastProgram(): base(null, FastNodeType.Program)
        {

        }

        public static FastProgram Compile(in StringSpan text)
        {
            var tokenStream = new FastTokenStream(text);
            var program = new FastProgram();
            program.Read(tokenStream);
            return program;
        }

    }
}
