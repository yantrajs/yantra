using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class FastScanner
    {

    }

    public readonly struct FastTokenType
    {

        public readonly int Id;

        

    }

    public readonly struct FastToken
    {
        public readonly FastTokenType Type;

        public readonly StringSpan Span;

        public FastToken(FastTokenType type, StringSpan span)
        {
            this.Type = type;
            this.Span = span;
        }

        public FastToken(FastTokenType type, string source, int start, int length)
        {
            this.Type = type;
            this.Span = new StringSpan(source, start, length);
        }

    }

    public class FastNode
    {

    }

    public class FastExpression
    {

    }

    public class FastBinaryExpression
    {

    }

    public class FastStatement
    {

    }

    public class FastBlock
    {
        public List<FastStatement> Statements;
    }

    public class FastProgram
    {



    }
}
