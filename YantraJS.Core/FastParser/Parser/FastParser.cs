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
                case TokenTypes.CurlyBracketEnd:
                    stream.Consume();
                    return true;
            }
            return false;
        }

    }
}
