using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core.FastParser
{

    public enum FastKeywords
    {
        none,
        let,
        var,
        @const,
        @for,
        @while,
        @do,
        @function,
        @if
    }

    public class FastKeywordMap
    {
        private static ConcurrentStringMap<FastKeywords> list = new ConcurrentStringMap<FastKeywords>();

        static FastKeywordMap()
        {
            foreach (var name in Enum.GetNames(typeof(FastKeywords)))
            {
                var value = (FastKeywords)Enum.Parse(typeof(FastKeywords), name);
                list[name] = value;
            }
        }

        public bool IsKeyword(in StringSpan k, out FastKeywords keyword)
        {
            return list.TryGetValue(k, out keyword);
        }
    }


    /// <summary>
    /// This class will provide stream of tokens, we are using this instead of
    /// scanner directly as we can move scanning process in different thread
    /// in future.
    /// </summary>
    public class FastTokenStream
    {
        private readonly FastScanner scanner;
        public readonly FastKeywordMap Keywords;
        private SparseList<FastToken> tokens;
        private int index;
        

        public FastTokenStream(in StringSpan text, FastKeywordMap keywords = null)
        {
            this.scanner = new FastScanner(text);
            tokens = new SparseList<FastToken>();
            index = 0;
            this.Keywords = keywords ?? new FastKeywordMap();
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

        public FastToken Expect(TokenTypes type)
        {
            var c = this[index];
            if (c.Type != type)
                throw new InvalidOperationException();
            Consume();
            return c;
        }

        public bool CheckAndConsumeKeywords(out FastKeywords keyword)
        {
            var c = this[index];
            if(c.Type == TokenTypes.Identifier)
            {
                if(Keywords.IsKeyword(in c.Span,out keyword))
                {
                    Consume();
                    return true;
                }
            }
            keyword = FastKeywords.none;
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


        public bool CheckAndConsume(FastKeywords keywords)
        {
            var c = this[index];
            if (c.Type == TokenTypes.Identifier)
            {
                if (Keywords.IsKeyword(in c.Span, out var k))
                {
                    if (k == keywords)
                    {
                        Consume();
                        return true;
                    }
                }
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
    }

    public class FastScanner
    {
        public readonly StringSpan Text;

        private int position = 0;

        private int line = 1;

        private int column = 1;

        private int templateParts = 0;

        public FastScanner(in StringSpan text)
        {
            this.Text = text;
        }

        

        private static FastToken EmptyToken = new FastToken(TokenTypes.Empty, string.Empty, 0, 0);
        private static FastToken EOF = new FastToken(TokenTypes.EOF, string.Empty, 0, 0);

        private FastToken token = EmptyToken;
        private FastToken nextToken = EOF;

        public FastToken Token
        {
            get
            {
                if(token.Type == 0)
                {
                    token = ReadToken();
                    nextToken = ReadToken();
                }
                return token;
            }
        }

        public void ConsumeToken()
        {
            token = nextToken;
            nextToken = ReadToken();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char Peek()
        {
            if (position >= Text.Length)
                return char.MaxValue;
            return Text[position];
        }

        private char Consume()
        {
            position++;
            if (position >= Text.Length)
                return char.MaxValue;
            char ch = Text[position];
            column++;
            if (ch == '\n')
            {
                line++;
                column = 0;
            }
            return ch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CanConsume(char ch)
        {
            if(ch == Peek()) { 
                Consume();
                return true;
            }
            return false;            
        }

        private FastToken ReadToken()
        {
            using (var state = Push())
            {
                char first = Peek();

                // if it is whitespace...
                // read all whitespace...
                while (char.IsWhiteSpace(first))
                {
                    first = Consume();
                    state.Reset();
                }

                if (first.IsIdentifierStart())
                {
                    return ReadIdentifier(state, first);
                }

                switch (first)
                {
                    case '\'':
                    case '"':
                        return ReadString(state, first);
                    case '`':
                        return ReadTemplateString(state);
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return ReadNumber(state, first);
                    case '/':
                        // Read comments
                        // Read Regex
                        // Read /=
                        return ReadCommentsOrRegExOrSymbol(state, first);
                    case ',':
                        return ReadSymbol(state, TokenTypes.Comma);
                    case '(':
                        return ReadSymbol(state, TokenTypes.BracketStart);
                    case ')':
                        return ReadSymbol(state, TokenTypes.BracketEnd);
                    case '[':
                        return ReadSymbol(state, TokenTypes.SquareBracketStart);
                    case ']':
                        return ReadSymbol(state, TokenTypes.SquareBracketEnd);
                    case '{':
                        return ReadSymbol(state, TokenTypes.CurlyBracketStart);
                    case '}':
                        if (templateParts > 0)
                        {
                            templateParts--;
                            return ReadTemplateString(state);
                        }
                        return ReadSymbol(state, TokenTypes.CurlyBracketEnd);
                    case '!':
                        Consume();
                        // !=
                        if (CanConsume('='))
                        {
                            // !==
                            if (CanConsume('='))
                                return state.Commit(TokenTypes.StrictlyNotEqual);
                            return state.Commit(TokenTypes.NotEqual);
                        }
                        return state.Commit(TokenTypes.Negate);
                    case '>':
                        Consume();
                        // >>
                        if(CanConsume('>'))
                        {
                            // >>>
                            if (CanConsume('>'))
                                return state.Commit(TokenTypes.UnsignedRightShift);
                            // >>=
                            if (CanConsume('='))
                                return state.Commit(TokenTypes.AssignRightShift);
                            return state.Commit(TokenTypes.RightShift);
                        }
                        // >=
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.GreaterOrEqual);
                        return state.Commit(TokenTypes.Greater);
                    case '<':
                        Consume();
                        // <<
                        if(CanConsume('<'))
                        {
                            // <<=
                            if (CanConsume('='))
                                return state.Commit(TokenTypes.AssignLeftShift);
                            return state.Commit(TokenTypes.LeftShift);
                        }
                        // <<=
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.LessOrEqual);
                        return state.Commit(TokenTypes.Less);
                    case '*':
                        Consume();
                        // **
                        if (CanConsume('*'))
                        {
                            // **=
                            if (CanConsume('='))
                                return state.Commit(TokenTypes.AssignPower);
                            return state.Commit(TokenTypes.Power);
                        }
                        return state.Commit(TokenTypes.Multiply);
                    case '&':
                        Consume();
                        if(CanConsume('&'))
                        {
                            return state.Commit(TokenTypes.BooleanAnd);
                        }
                        return state.Commit(TokenTypes.BitwiseAnd);
                    case '|':
                        Consume();
                        if (CanConsume('|'))
                            return state.Commit(TokenTypes.BooleanOr);
                        return state.Commit(TokenTypes.BitwiseOr);
                    case '+':
                        Consume();
                        if (CanConsume('+'))
                            return state.Commit(TokenTypes.Increment);
                        return state.Commit(TokenTypes.Plus);
                    case '-':
                        Consume();
                        if (CanConsume('-'))
                            return state.Commit(TokenTypes.Decrement);
                        return state.Commit(TokenTypes.Minus);
                    case '^':
                        Consume();
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignXor);
                        return state.Commit(TokenTypes.Xor);
                    case '?':
                        return ReadSymbol(state, TokenTypes.QuestionMark);
                    case ':':
                        return ReadSymbol(state, TokenTypes.Colon);
                    case ';':
                        return ReadSymbol(state, TokenTypes.SemiColon);
                    case '~':
                        return ReadSymbol(state, TokenTypes.BitwiseNot);
                    case '%':
                        Consume();
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignMod);
                        return state.Commit(TokenTypes.Mod);
                    case '=':
                        Consume();
                        if(CanConsume('='))
                        {
                            if(CanConsume('='))
                            {
                                return state.Commit(TokenTypes.StrictlyEqual);
                            }
                            return state.Commit(TokenTypes.Equal);
                        }
                        if(CanConsume('>'))
                        {
                            return state.Commit(TokenTypes.Lambda);
                        }
                        return state.Commit(TokenTypes.Assign);
                        
                }
                
            }

            return EOF;
        }

        private FastToken ReadTemplateString(State state)
        {
            do {
                char ch = Consume();
                if(ch == '$')
                {
                    if(CanConsume('{'))
                    {
                        // template part begin...
                        if(templateParts++ == 0)
                            return state.Commit(TokenTypes.TemplateBegin);
                        return state.Commit(TokenTypes.TemplatePart);
                    }
                }
                if (ch == char.MaxValue)
                    break;
                if (CanConsume('`'))
                    return state.Commit(TokenTypes.TemplateEnd);
            } while (true);
            throw new InvalidOperationException();
        }

        private FastToken ReadSymbol(State state, TokenTypes type)
        {
            Consume();
            return state.Commit(type);
        }

        private FastToken ReadCommentsOrRegExOrSymbol(State state, char first)
        {
            first = Consume();
            bool mayBeLambda = false;
            switch (first)
            {
                case '/': 
                    return SkipSingleLineComment(state);
                case '*':
                    return SkipMultilineComment(state);
                case '=':
                    // this case should first consider if it is part of Regex or not..
                    mayBeLambda = true;
                    break;
            }

            if(mayBeLambda)
            {
                Consume();
                return state.Commit(TokenTypes.Lambda);
            }

            throw new NotImplementedException($"{first} not supported");
        }

        private FastToken SkipMultilineComment(State state)
        {
            char ch;
            do
            {
                ch = Consume();
                if (ch == char.MaxValue)
                    break;
                if (ch == '*')
                {
                    ch = Consume();
                    if (ch == char.MaxValue || ch == '/')
                        break;
                }
            } while (true);
            Consume();
            state.Reset();
            return ReadToken();
        }

        private FastToken SkipSingleLineComment(State state)
        {
            char ch;
            do
            {
                ch = Consume();
            } while (ch != '\n' && ch != char.MaxValue);
            Consume();
            state.Reset();
            return ReadToken();
        }

        private FastToken ReadString(State state, char first)
        {
            var start = first;
            do
            {
                Consume();
                first = Peek();
                if (first == '\\')
                {
                    Consume();
                    var next = Peek();
                    if (next == first)
                    {
                        continue;
                    }
                }
                if (first == start)
                {
                    Consume();
                    var next = Peek();
                    if (next == first)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            } while (first != start);
            return state.Commit(TokenTypes.String);
        }

        private FastToken ReadIdentifier(State state, char first)
        {
            do
            {
                Consume();
                first = Peek();

            } while (first.IsIdentifierPart());
            return state.Commit(TokenTypes.Identifier);
        }

        private FastToken ReadNumber(State state, char first)
        {
            var isZero = first == '0';
            var isHex = false;
            var isBinary = false;
            var readDecimal = true;
            do
            {
                Consume();
                first = Peek();
                if (isZero)
                {
                    isZero = false;
                    isHex = first == 'x';
                    isBinary = first == 'b';
                    if (isHex || isBinary)
                    {
                        Consume();
                        first = Peek();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if(first == '.')
                {
                    readDecimal = false;
                    continue;
                }
            } while (first.IsDigitPart(isHex, isBinary, readDecimal));
            return state.Commit(TokenTypes.Number);
        }


        public State Push()
        {
            return new State(this, position, line, column);
        }
        
        public class State: IDisposable
        {
            private FastScanner scanner;
            private int position;
            private int line;
            private int column;

            public State(FastScanner scanner, int position, int line, int column)
            {
                this.scanner = scanner;
                this.line = line;
                this.column = column;
                this.position = position;
            }

            public FastToken Commit(TokenTypes type)
            {
                var cp = scanner.position;
                var start = scanner.Text.Offset + position;
                var token = new FastToken(type, scanner.Text.Source, start, cp - start);
                scanner = null;
                return token;
            }

            public void Reset()
            {
                position = scanner.position;
                line = scanner.line;
                column = scanner.column;
            }

            public void Dispose()
            {
                if (scanner != null)
                {
                    scanner.position = position;
                    scanner.line = line;
                    scanner.column = column;
                }
            }
        }

    }
}
