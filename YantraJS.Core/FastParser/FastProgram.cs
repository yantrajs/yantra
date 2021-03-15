using System.Linq;
using System.Text;

namespace YantraJS.Core.FastParser
{


    public class FastProgram: FastBlock {

        public FastProgram(FastTokenStream stream): base(null, FastNodeType.Program, stream)
        {
            
        }

        public static FastProgram Compile(in StringSpan text)
        {
            var tokenStream = new FastTokenStream(text);
            return new FastProgram(tokenStream);
        }

    }
}
