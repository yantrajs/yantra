using System;

namespace YantraJS.Core.FastParser
{
    /// <summary>
    /// This class will provide stream of tokens, we are using this instead of
    /// scanner directly as we can move scanning process in different thread
    /// in future.
    /// </summary>
    public class FastTokenStream
    {
        private readonly FastScanner scanner;

        internal Exception Unexpected()
        {
            var c = Current;
            return new FastParseException(c, $"Unexpected token {c.Type}: {c.Span} at {scanner.Location}");
        }

        public readonly FastKeywordMap Keywords;
        private SparseList<FastToken> tokens;
        private int index;

        public FastTokenStream(in StringSpan text, FastKeywordMap keywords = null)
        {
            tokens = new SparseList<FastToken>();
            index = 0;
            this.Keywords = keywords ?? FastKeywordMap.Instance;
            this.scanner = new FastScanner(text, Keywords);
        }

        private FastToken this[int index]
        {
            get
            {
                while (tokens.Count <= index)
                {
                    tokens.Add(scanner.Token);
                    scanner.ConsumeToken();
                }
                return tokens[index];
            }
        }

        public FastToken Current => this[index];

        public FastToken Next => this[index + 1];

        public FastToken Previous => this[index > 0 ? index - 1 : index];

        public FastToken Expect(TokenTypes type)
        {
            var c = this[index];
            if (c.Type != type)
                throw new InvalidOperationException();
            Consume();
            return c;
        }

        public FastToken Expect(FastKeywords type)
        {
            var c = this[index];
            if (c.Keyword != type)
                throw new FastParseException(c, $"Expecting keyword {type}");
            Consume();
            return c;
        }


        public bool CheckAndConsumeContextualKeyword(FastKeywords keyword)
        {
            var c = this[index];
            if (c.ContextualKeyword == keyword)
            {
                Consume();
                return true;
            }
            return false;
        }


        public bool CheckAndConsume(FastKeywords keyword)
        {
            var c = this[index];
            if (c.Keyword == keyword)
            {
                Consume();
                return true;
            }
            return false;
        }


        public bool CheckAndConsume(TokenTypes type)
        {
            var c = this[index];
            if (c.Type == type)
            {
                Consume();
                return true;
            }
            return false;
        }

        public bool CheckAndConsume(TokenTypes type, out FastToken token)
        {
            var c = this[index];
            if (c.Type == type)
            {
                token = c;
                Consume();
                return true;
            }
            token = null;
            return false;
        }

        public FastToken Consume()
        {
            index++;
            return this[index];
        }

        public CancellableDisposableAction UndoMark()
        {
            var i = index;
            return new CancellableDisposableAction(() => {
                index = i;
            });
        }

        public int Position => index;

        public bool Reset(int position)
        {
            index = position;
            return false;
        }
    }
}
