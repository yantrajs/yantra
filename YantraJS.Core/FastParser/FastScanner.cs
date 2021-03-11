using System;

namespace YantraJS.Core.FastParser
{

    public class FastScanner
    {
        public readonly StringSpan Text;

        private int position = 0;

        private int line = 1;

        private int column = 1;

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

        private FastToken Match(
            State state, 
            TokenTypes def, 
            char ch1, 
            TokenTypes type1, 
            char ch2, 
            TokenTypes type2)
        {
            char ch = Consume();
            if (ch == ch1)
            {
                ch = Consume();
                if (ch == ch2)
                {
                    Consume();
                    return state.Commit(type2);
                }
                return state.Commit(type1);
            }
            return state.Commit(def);
        }

        private bool PeekConsume(char ch)
        {
            var c = Peek();
            if(ch == c)
            {
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
                    case '(':
                        return ReadSymbol(state, TokenTypes.BracketStart);
                    case ')':
                        return ReadSymbol(state, TokenTypes.BracketEnd);
                    case '!':
                        Consume();
                        // !=
                        if (PeekConsume('='))
                        {
                            // !==
                            if (PeekConsume('='))
                                return state.Commit(TokenTypes.StrictlyNotEqual);
                            return state.Commit(TokenTypes.NotEqual);
                        }
                        return state.Commit(TokenTypes.Negate);
                    case '>':
                        Consume();
                        // >>
                        if(PeekConsume('>'))
                        {
                            // >>>
                            if (PeekConsume('>'))
                                return state.Commit(TokenTypes.UnsignedRightShift);
                            // >>=
                            if (PeekConsume('='))
                                return state.Commit(TokenTypes.AssignRightShift);
                            return state.Commit(TokenTypes.RightShift);
                        }
                        // >=
                        if (PeekConsume('='))
                            return state.Commit(TokenTypes.GreaterOrEqual);
                        return state.Commit(TokenTypes.Greater);
                    case '<':
                        Consume();
                        // <<
                        if(PeekConsume('<'))
                        {
                            // <<=
                            if (PeekConsume('='))
                                return state.Commit(TokenTypes.AssignLeftShift);
                            return state.Commit(TokenTypes.LeftShift);
                        }
                        // <<=
                        if (PeekConsume('='))
                            return state.Commit(TokenTypes.LessOrEqual);
                        return state.Commit(TokenTypes.Less);
                    case '*':
                        Consume();
                        // **
                        if (PeekConsume('*'))
                        {
                            // **=
                            if (PeekConsume('='))
                                return state.Commit(TokenTypes.AssignPower);
                            return state.Commit(TokenTypes.Power);
                        }
                        return state.Commit(TokenTypes.Multiply);
                    case '&':
                        Consume();
                        if(PeekConsume('&'))
                        {
                            return state.Commit(TokenTypes.BooleanAnd);
                        }
                        return state.Commit(TokenTypes.BitwiseAnd);
                    case '|':
                        Consume();
                        if (PeekConsume('|'))
                            return state.Commit(TokenTypes.BooleanOr);
                        return state.Commit(TokenTypes.BitwiseOr);
                    case '+':
                        Consume();
                        if (PeekConsume('+'))
                            return state.Commit(TokenTypes.Increment);
                        return state.Commit(TokenTypes.Plus);
                    case '-':
                        Consume();
                        if (PeekConsume('-'))
                            return state.Commit(TokenTypes.Decrement);
                        return state.Commit(TokenTypes.Minus);
                    case '^':
                        Consume();
                        if (PeekConsume('='))
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
                        if (PeekConsume('='))
                            return state.Commit(TokenTypes.AssignMod);
                        return state.Commit(TokenTypes.Mod);
                    case '=':
                        Consume();
                        if(PeekConsume('='))
                        {
                            if(PeekConsume('='))
                            {
                                return state.Commit(TokenTypes.StrictlyEqual);
                            }
                            return state.Commit(TokenTypes.Equal);
                        }
                        if(PeekConsume('>'))
                        {
                            return state.Commit(TokenTypes.Lambda);
                        }
                        return state.Commit(TokenTypes.Assign);
                        
                }
                
            }

            return EOF;
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
        
        public struct State: IDisposable
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
