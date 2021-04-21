using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{

    /// <summary>
    /// Scanner Features.
    /// 
    /// 1.  Scanner ignores whitespace and comments
    ///     but a token is marked as LineTerminated if it is
    ///     followed by a line terminator.
    ///     
    ///     This is useful in the case when expression needs
    ///     a line terminator as a expression end marker.
    ///     
    ///     Ignoring line terminator and whitespace makes
    ///     parsing rules simple as everything else are pure
    ///     tokens.
    ///     
    /// 2.  Scanner parses first token and keeps next token 
    ///     ready. Only when you consume current token, next 
    ///     token is read. This is to avoid in case of failure.
    ///     
    /// 3.  Never read beyond EOF, because once you encounter
    ///     EOF, scanner will endlessly send you EOF. It is 
    ///     responsibility of the Parser to detect end of program.
    /// 
    /// </summary>
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

            token = ReadToken();
            nextToken = ReadToken();
        }

        

        private static readonly FastToken EmptyToken = new FastToken(TokenTypes.Empty, string.Empty);
        private static readonly FastToken EOF = new FastToken(TokenTypes.EOF, string.Empty);

        private FastToken token = EmptyToken;
        private FastToken nextToken = EOF;

        private FastToken lastToken = EmptyToken;

        public FastToken Token
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return token;
            }
        }

        public void ConsumeToken()
        {
            // lets ignore consecutive line terminators
            token = nextToken;
            nextToken = ReadToken();
            while(token.Type == TokenTypes.LineTerminator && nextToken.Type == TokenTypes.LineTerminator)
            {
                token = nextToken;
                nextToken = ReadToken();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char Peek()
        {
            if (position >= Text.Length)
                return char.MaxValue;
            return Text[position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char Next() {
            var next = position + 1;
            if (next >= Text.Length)
                return char.MaxValue;
            return Text[next];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char Consume()
        {
            if (position >= Text.Length)
                return char.MaxValue;
            char ch = Text[position];
            if(ch == '\n') {
                line++;
                column = 0;
            } else {
                column++;
            }
            position++;
            if (position >= Text.Length)
                return char.MaxValue;
            ch = Text[position];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CanConsume(char ch1, char ch2) {
            var ch = Peek();
            if (ch == ch1 || ch == ch2) {
                Consume();
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FastToken ReadToken()
        {
            lastToken = _ReadToken();
            return lastToken;
        }

        private FastToken _ReadToken()
        {
            var state = Push();
            
            char first = Peek();
            if (first == char.MaxValue)
            {
                return EOF;
            }

            // if it is whitespace...
            // read all whitespace...
            while (char.IsWhiteSpace(first))
            {
                if(first == '\n')
                {
                    return ReadSymbol(state, TokenTypes.LineTerminator);
                }
                first = Consume();
                state = Push();
            }

            if (first.IsIdentifierStart())
            {
                return ReadIdentifier(state);
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
                    return ReadCommentsOrRegExOrSymbol(state);
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
                        return ReadTemplateString(state, TokenTypes.TemplatePart);
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
                    var peek = Next();
                    if (char.IsDigit(peek)) {
                        Consume();
                        return ReadNumber(state, first);
                    }
                    Consume();
                    if (CanConsume('.'))
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
                
            

            return EOF;
        }

        private bool ScanEscaped(char next, StringBuilder t)
        {
            if (next != '\\')
                return false;
            next = Consume();
            switch (next)
            {
                /**
                 * This is special case, slash followed by a single line terminator is
                 * only used to break the string starting at next line
                 */
                case '\n':
                    return true;
                case 'u':
                    if(CanConsume('{'))
                    {
                        t.Append(ScanUnicodeCodePointEscape());
                        return true;
                    }
                    if(ScanHexEscape(next, out var n))
                    {
                        t.Append(n);
                        return true;
                    }
                    throw Unexpected();
                case 'n':
                    next = '\n';
                    break;
                case 'r':
                    next = '\r';
                    break;
                case 't':
                    next = '\t';
                    break;
                case 'b':
                    next = '\b';
                    break;
                case 'f':
                    next = '\f';
                    break;
                case 'v':
                    next = '\v';
                    break;
                default:
                    t.Append(next);
                    return true;
            }
            t.Append(next);
            return true;

            string ScanUnicodeCodePointEscape()
            {
                var ch = Consume();
                int code = 0;

                // At least, one hex digit is required.
                if (ch == '}')
                {
                    throw Unexpected();
                }

                while (ch != char.MaxValue)
                {
                    if (!ch.IsDigitPart(true,false))
                    {
                        break;
                    }
                    code = code * 16 + ch.HexValue();
                    ch = Consume();
                }

                if (code > 0x10FFFF || ch != '}')
                {
                    throw Unexpected();
                }

                return code.FromCodePoint();
            }

            bool ScanHexEscape(char prefix, out char result)
            {
                var len = (prefix == 'u') ? 4 : 2;
                var code = 0;

                for (var i = 0; i < len; ++i)
                {
                    char ch = Consume();
                    if (ch != char.MaxValue)
                    {
                        if (ch.IsDigitPart(true, false))
                        {
                            code = code * 16 + ch.HexValue();
                        }
                        else
                        {
                            result = char.MinValue;
                            return false;
                        }
                    }
                    else
                    {
                        result = char.MinValue;
                        return false;
                    }
                }

                result = (char)code;
                return true;
            }
        }

        private FastToken ReadTemplateString(State state, TokenTypes part = TokenTypes.TemplateBegin)
        {
            var sb = pool.AllocateStringBuilder();
            var t = sb.Builder;
            try
            {
                do
                {
                    char ch = Consume();
                    switch(ch)
                    {
                        case '$':
                                Consume();
                                if (CanConsume('{')) {
                                    // template part begin...
                                    templateParts++;
                                    return state.Commit(part, t);
                                    // return state.Commit(TokenTypes.TemplatePart, t);
                                }
                                t.Append(ch);
                                continue;
                        case '`':
                            Consume();
                            return state.Commit(TokenTypes.TemplateEnd, t);
                        case char.MaxValue:
                            break;
                    }
                    if (ch == char.MaxValue)
                        throw Unexpected();
                    if (ScanEscaped(ch, t))
                        continue;
                    t.Append(ch);
                } while (true);
            } finally
            {
                sb.Clear();
            }
        }

        private FastToken ReadSymbol(State state, TokenTypes type)
        {
            Consume();
            return state.Commit(type);
        }

        private FastToken ReadCommentsOrRegExOrSymbol(State state)
        {
            var divide = Push();
            var first = Consume();
            bool divideAndAssign = false;
            switch (first)
            {
                /**
                 * '//'
                 */
                case '/': 
                    return SkipSingleLineComment(state);
                /**
                 * '/*'
                 */
                case '*':
                    return SkipMultilineComment(state);
                /**
                 * '/='
                 */
                case '=':
                    // this case should first consider if it is part of Regex or not..
                    divideAndAssign = true;
                    break;
            }

            if (ScanRegEx(state, first, out var token))
                return token;
            if(divideAndAssign)
            {
                state.Dispose();
                Consume();
                Consume();
                return divide.Commit(TokenTypes.AssignDivide);
            }

            state.Dispose();
            Consume();
            return divide.Commit(TokenTypes.Divide);

            bool ScanRegEx(State state, char first , out FastToken token)
            {
                /**
                    * Regex will never be followed by 
                    * `)`, `]` and `keyword or identifier`
                    */
                switch (lastToken.Type)
                {

                    case TokenTypes.Identifier:
                        if(!lastToken.IsKeyword)
                        {
                            token = null;
                            return false;
                        }
                        break;
                    case TokenTypes.BracketEnd:
                    case TokenTypes.SquareBracketEnd:
                        token = null;
                        return false;
                }

                var sb = pool.AllocateStringBuilder();
                var t = sb.Builder;
                var classMarker = false;
                var terminated = false;
                token = null;
                string regExp = null;
                try
                {
                    do
                    {
                        switch (first)
                        {
                            case char.MaxValue:
                                return false; 
                            case '\n':
                                return false;
                            case '/':
                                if(classMarker)
                                {
                                    t.Append(first);
                                    break;
                                }
                                terminated = true;
                                Consume();
                                break;
                            case '[':
                                classMarker = true;
                                t.Append(first);
                                break;
                            case ']':
                                classMarker = false;
                                t.Append(first);
                                break;
                            case '\\':
                                //if (ScanEscaped(first, t))
                                //    continue;
                                first = Consume();
                                if (first == '/')
                                {
                                    t.Append('\\');
                                    t.Append('/');
                                    break;
                                }
                                if (CanConsume('\n'))
                                {
                                    return false;
                                }

                                t.Append('\\');
                                t.Append(first);
                                break;
                            default:
                                t.Append(first);
                                break;
                        }
                        if (terminated)
                            break;
                        first = Consume();
                    } while (true);

                    regExp = t.ToString();
                }
                finally
                {
                    sb.Clear();
                }

                var flags = ScanFlags();

                // we should test if it is a valid JSRegEx
                var (r,_,_,_) = JSRegExp.CreateRegex(regExp, flags);
                if (r == null)
                    return false;


                token = state.Commit(TokenTypes.RegExLiteral, regExp, flags);

                return true;
            }

            string ScanFlags()
            {
                var sb = pool.AllocateStringBuilder();
                var t = sb.Builder;
                var d = false;
                var g = false;
                var i = false;
                var m = false;
                var s = false;
                var u = false;
                var y = false;
                try
                {
                    do
                    {
                        var ch = Peek();
                        switch(ch)
                        {
                            case 'd':
                                if (d) throw Unexpected();
                                d = true;
                                t.Append(ch);
                                Consume();
                                continue;
                            case 'g':
                                if (g) throw Unexpected();
                                g = true;
                                t.Append(ch);
                                Consume();
                                continue;
                            case 'i':
                                if (i) throw Unexpected();
                                i = true;
                                t.Append(ch);
                                Consume();
                                continue;
                            case 'm':
                                if (m) throw Unexpected();
                                m = true;
                                t.Append(ch);
                                Consume();
                                continue;
                            case 's':
                                if (s) throw Unexpected();
                                s = true;
                                t.Append(ch);
                                Consume();
                                continue;
                            case 'u':
                                if (u) throw Unexpected();
                                u = true;
                                t.Append(ch);
                                Consume();
                                continue;
                            case 'y':
                                if (y) throw Unexpected();
                                y = true;
                                t.Append(ch);
                                Consume();
                                continue;
                        }
                        break;
                    } while (true);
                    return sb.ToString();
                } finally
                {
                    sb.Clear();
                }
            }
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
                    if (ch == '/') {
                        Consume();
                        break;
                    }
                    if (ch == char.MaxValue) {
                        break;
                    }
                }
            } while (true);
            return ReadToken();
        }

        private FastToken SkipSingleLineComment(State state)
        {
            char ch;
            do
            {
                ch = Consume();
            } while (ch != '\n' && ch != char.MaxValue);
            // Consume();
            return ReadSymbol(state, TokenTypes.LineTerminator);
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
                    if (ScanEscaped(first, t))
                        continue;
                    t.Append(first);
                    if (first == start)
                        break;
                } while (true);
                return state.Commit(TokenTypes.String, sb.Builder);
            } finally {
                sb.Clear();
            }
        }

        private FastToken ReadIdentifier(State state)
        {
            char first;
            do
            {
                first = Consume();

            } while (first.IsIdentifierPart());
            var token = state.CommitIdentifier(keywords);
            return token;
        }

        private FastToken ReadNumber(State state, char first)
        {
            void ConsumeDigits(bool hex = false, bool binary = false) {
                char peek = Peek();
                if (!peek.IsDigitPart(hex, binary))
                    return;
                do {
                    peek = Consume();
                } while (peek.IsDigitPart(hex, binary));
            }
            if(CanConsume('0')) {
                if(CanConsume('x', 'X')) {
                    ConsumeDigits(hex: true);
                    return state.Commit(TokenTypes.Number, true);
                }
                if(CanConsume('b','B')) {
                    ConsumeDigits(binary: true);
                    return state.Commit(TokenTypes.Number, true);
                }
            }
            ConsumeDigits();
            if (CanConsume('.')) {
                ConsumeDigits();
            }
            if (CanConsume('e','E')) {
                if (CanConsume('+', '-')) {
                    ConsumeDigits();
                    return state.Commit(TokenTypes.Number, true);
                }
                ConsumeDigits();
                return state.Commit(TokenTypes.Number, true);
            }
            ConsumeDigits();
            return state.Commit(TokenTypes.Number, true);
        }


        public State Push()
        {
            return new State(this, position, line, column);
        }
        
        public struct State
        {
            private FastScanner scanner;
            private int position;
            private SpanLocation start;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public State(FastScanner scanner, int position, int line, int column)
            {
                this.scanner = scanner;
                this.start = scanner.Location;
                this.position = position;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FastToken Commit(TokenTypes type, string cooked, string flags)
            {
                var cp = scanner.position;
                var start = scanner.Text.Offset + position;
                var location = scanner.Location;
                var token = new FastToken(
                    type,
                    scanner.Text.Source,
                    cooked,
                    flags,
                    start, cp - start,
                    this.start,
                    location);
                scanner = null;
                return token;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FastToken Commit(TokenTypes type, bool number) {
                var cp = scanner.position;
                var start = scanner.Text.Offset + position;
                var location = scanner.Location;
                var token = new FastToken(
                    type,
                    scanner.Text.Source,
                    null,
                    null,
                    start, cp - start,
                    this.start,
                    location,
                    number);
                scanner = null;
                return token;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FastToken Commit(TokenTypes type, StringBuilder builder = null)
            {
                var cp = scanner.position;
                var start = scanner.Text.Offset + position;
                var location = scanner.Location;
                var token = new FastToken(
                    type,
                    scanner.Text.Source,
                    builder?.ToString(),
                    null,
                    start, cp - start,
                    this.start,
                    location);
                scanner = null;
                return token;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                position = scanner.position;
                start = scanner.Location;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                if (scanner != null)
                {
                    scanner.position = position;
                    scanner.line = start.Line;
                    scanner.column = start.Column;
                    scanner = null;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal FastToken CommitIdentifier(FastKeywordMap keywords)
            {


                var cp = scanner.position;
                var start = scanner.Text.Offset + position;
                var location = scanner.Location;
                var token = new FastToken(
                    TokenTypes.Identifier,
                    scanner.Text.Source,
                    null,
                    null,
                    start, cp - start,
                    this.start,
                    location, false, keywords);
                scanner = null;
                return token;
            }
        }
    }
}
