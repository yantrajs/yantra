using System;
using System.Runtime.CompilerServices;

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
        public readonly FastPool Pool;

        internal Exception Unexpected()
        {
            var c = Current;
            return new FastParseException(c, $"Unexpected token {c.Type}: {c.Span} at {c.Start}");
        }

        public override string ToString()
        {
            return $"{Current} {Next}";
        }

        public readonly FastKeywordMap Keywords;
        private readonly SparseList<FastToken> tokens;
        private int index;

        public FastTokenStream(in StringSpan text, FastKeywordMap keywords = null)
        {
            this.Pool = new FastPool();
            tokens = new SparseList<FastToken>();
            index = 0;
            this.Keywords = keywords ?? FastKeywordMap.Instance;
            this.scanner = new FastScanner(Pool, text, Keywords);
        }


        public FastTokenStream(FastPool pool, in StringSpan text, FastKeywordMap keywords = null)
        {
            this.Pool = pool;
            tokens = new SparseList<FastToken>();
            index = 0;
            this.Keywords = keywords ?? FastKeywordMap.Instance;
            this.scanner = new FastScanner(pool, text, Keywords);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastToken Expect(TokenTypes type)
        {
            var c = this[index];
            if (c.Type != type)
                throw new FastParseException(c, $"Expecting {type} at {c.Start.Line}, {c.Start.Column}");
            Consume();
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastToken Expect(FastKeywords type)
        {
            var c = this[index];
            if (c.Keyword != type)
                throw new FastParseException(c, $"Expecting keyword {type} at {c.Start.Line}, {c.Start.Column}");
            Consume();
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastToken ExpectContextualKeyword(FastKeywords type)
        {
            var c = this[index];
            if (c.ContextualKeyword != type)
                throw new FastParseException(c, $"Expecting keyword {type} at {c.Start.Line}, {c.Start.Column}");
            Consume();
            return c;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckAndConsumeAny(TokenTypes type1, TokenTypes type2)
        {
            var c = this[index].Type;
            if (c == type1 ||  c == type2)
            {
                Consume();
                return true;
            }
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckAndConsumeAny(TokenTypes type1, TokenTypes type2, TokenTypes type3)
        {
            var c = this[index].Type;
            if (c == type1 || c == type2 || c == type3)
            {
                Consume();
                return true;
            }
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckAndConsumeAny(TokenTypes type1, TokenTypes type2, TokenTypes type3, TokenTypes type4)
        {
            var ct = this[index].Type;
            if (ct == type1 || ct == type2 || ct == type3 || ct == type4)
            {
                Consume();
                return true;
            }
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckAndConsume(TokenTypes type1, TokenTypes type2, out FastToken token1, out FastToken token2)
        {
            var c = this[index];
            if (c.Type == type1)
            {
                token1 = c;
                c = this[index + 1];
                if(c.Type == type2)
                {
                    Consume();
                    Consume();
                    token2 = c;
                    return true;
                }
            }
            token1 = null;
            token2 = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return index;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Reset(int position)
        {
            index = position;
            return false;
        }
    }
}
