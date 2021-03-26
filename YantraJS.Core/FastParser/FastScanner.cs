using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public class FastScanner
    {
        private readonly FastPool pool;
        public readonly StringSpan Text;
        private readonly FastKeywordMap keywords;
        private int position = 0;

        private int line = 1;

        private int column = 1;

        private int templateParts = 0;

        public SpanLocation Location => new SpanLocation(line, column);

        public Exception Unexpected()
        {
            var c = this.token;
            return new FastParseException(c, $"Unexpected token {c.Type}: {c.Span} at {Location}");
        }

        public FastScanner(FastPool pool, in StringSpan text, FastKeywordMap keywords = null)
        {
            this.pool = pool;
            this.Text = text;
            this.keywords = keywords ?? FastKeywordMap.Instance;
        }

        

        private static FastToken EmptyToken = new FastToken(TokenTypes.Empty, string.Empty, null, 0, 0, 0, 0, 0, 0);
        private static FastToken EOF = new FastToken(TokenTypes.EOF, string.Empty, null, 0, 0, 0, 0, 0, 0);

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
                    case '#':
                        return ReadSymbol(state, TokenTypes.Hash);
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
                            {
                                if (CanConsume('='))
                                    return state.Commit(TokenTypes.AssignUnsignedRightShift);
                                return state.Commit(TokenTypes.UnsignedRightShift);
                            }
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
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignMultiply);
                        return state.Commit(TokenTypes.Multiply);
                    case '&':
                        Consume();
                        if(CanConsume('&'))
                        {
                            return state.Commit(TokenTypes.BooleanAnd);
                        }
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignBitwideAnd);
                        return state.Commit(TokenTypes.BitwiseAnd);
                    case '|':
                        Consume();
                        if (CanConsume('|'))
                            return state.Commit(TokenTypes.BooleanOr);
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignBitwideOr);
                        return state.Commit(TokenTypes.BitwiseOr);
                    case '+':
                        Consume();
                        if (CanConsume('+'))
                            return state.Commit(TokenTypes.Increment);
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignAdd);
                        return state.Commit(TokenTypes.Plus);
                    case '-':
                        Consume();
                        if (CanConsume('-'))
                            return state.Commit(TokenTypes.Decrement);
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignSubtract);
                        return state.Commit(TokenTypes.Minus);
                    case '^':
                        Consume();
                        if (CanConsume('='))
                            return state.Commit(TokenTypes.AssignXor);
                        return state.Commit(TokenTypes.Xor);
                    case '?':
                        Consume();
                        if (CanConsume('.'))
                            return state.Commit(TokenTypes.QuestionDot);
                        return state.Commit(TokenTypes.QuestionMark);
                    case '.':
                        Consume();
                        if(CanConsume('.'))
                        {
                            if(CanConsume('.'))
                            {
                                return state.Commit(TokenTypes.TripleDots);
                            }
                            throw Unexpected();
                        }
                        return state.Commit(TokenTypes.Dot);
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
                    case '\n':
                        return ReadSymbol(state, TokenTypes.LineTerminator);
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
            var sb = pool.AllocateStringBuilder();
            var t = sb.Builder;
            try
            {
                do
                {
                    first = Consume();
                    if (first == '\\')
                    {
                        var next = Consume();
                        if (next == first)
                        {
                            t.Append(first);
                            continue;
                        }
                        t.Append(first);
                        t.Append(next);
                    }
                    if (first == start)
                    {
                        var next = Consume();
                        if (next == first)
                        {
                            t.Append(first);
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    t.Append(first);
                } while (first != start);
                return state.Commit(TokenTypes.String, sb.Builder);
            } finally {
                sb.Clear();
            }
        }

        private FastToken ReadIdentifier(State state, char first)
        {
            do
            {
                Consume();
                first = Peek();

            } while (first.IsIdentifierPart());
            var token = state.CommitIdentifier(keywords);
            return token;
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

            public FastToken Commit(TokenTypes type, StringBuilder builder = null)
            {
                var cp = scanner.position;
                var start = scanner.Text.Offset + position;
                var token = new FastToken(
                    type, 
                    scanner.Text.Source, 
                    builder,
                    start, cp - start, 
                    line,
                    column,
                    scanner.line, 
                    scanner.column);
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

            internal FastToken CommitIdentifier(FastKeywordMap keywords)
            {
                var cp = scanner.position;
                var start = scanner.Text.Offset + position;
                var token = new FastToken(
                    TokenTypes.Identifier,
                    scanner.Text.Source,
                    null,
                    start, cp - start,
                    line,
                    column,
                    scanner.line,
                    scanner.column, keywords);
                scanner = null;
                return token;
            }
        }

    }
}
