using System;
using System.Collections.Concurrent;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public partial class FastParser
    {
        private readonly FastTokenStream stream;

        public readonly FastPool Pool = new FastPool();


        public StreamLocation Location => new StreamLocation(this, stream.Position, stream.Current);

        public FastToken PreviousToken => stream.Previous;

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

        }

        public AstProgram ParseProgram()
        {
            if (Program(out var p))
                return p;
            throw stream.Unexpected();
        }

        private Func<T> Consume<T>(Func<T> func)
        {
            return () => {
                stream.Consume();
                return func();
            };
        }

        bool EndOfStatement()
        {
            var token = stream.Current;
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
