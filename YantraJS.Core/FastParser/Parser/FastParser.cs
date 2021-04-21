using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public partial class FastParser
    {
        private readonly FastTokenStream stream;

        public readonly FastPool Pool;

        public readonly FastScope variableScope;

        /// <summary>
        /// Disable this inside for brackets...
        /// </summary>
        private bool considerInOfAsOperators = true;

        public StreamLocation BeginUndo() => new StreamLocation(this, stream.Position, stream.Current);


        public StreamLocation Location
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new StreamLocation(this, stream.Position, stream.Current);
            }
        }

        public FastToken PreviousToken
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return stream.Previous;
            }
        }

        public readonly struct StreamLocation
        {
            private readonly FastParser parser;
            private readonly int position;
            public readonly FastToken Token;

            public StreamLocation(FastParser parser, int index, FastToken token)
            {
                this.parser = parser;
                this.position = index;
                this.Token = token;
            }

            public bool Reset()
            {
                parser.stream.Reset(position);
                return false;
            }
        }

        public FastParser(FastTokenStream stream)
        {
            this.stream = stream;
            this.Pool = stream.Pool;
            this.variableScope = new FastScope(Pool);
        }

        public AstProgram ParseProgram()
        {
            if (Program(out var p))
                return p;
            throw stream.Unexpected();
        }

        bool EndOfLine()
        {
            var token = stream.Current;
            if(token.Type == TokenTypes.LineTerminator)
            {
                stream.Consume();
                return true;
            }
            return false;
        }

        bool EndOfStatement()
        {
            var token = stream.Current;
            //if (token.LineTerminator)
            //    return true;
            switch (token.Type)
            {
                case TokenTypes.SemiColon:
                case TokenTypes.EOF:
                case TokenTypes.LineTerminator:
                    stream.Consume();
                    return true;
                // since Block will expect curly bracket
                // to be present, we will not consume this..
                case TokenTypes.CurlyBracketEnd:
                    return true;
            }
            return false;
        }

    }
}
