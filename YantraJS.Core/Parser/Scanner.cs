using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Parser
{
    //public partial class Global
    //{
    //    public static int HexValue(string ch)
    //    {
    //        return "0123456789abcdef".IndexOf(ch.ToLower());
    //    }
    //}
    //public partial class Global
    //{
    //    public static int OctalValue(string ch)
    //    {
    //        return "01234567".IndexOf(ch);
    //    }
    //}
    //public class Position
    //{
    //    public int line;
    //    public int column;
    //}
    //public class SourceLocation
    //{
    //    public Position start;
    //    public Position end;
    //    public string source;
    //}
    //public class Comment
    //{
    //    public bool multiLine;
    //    public List<int> slice;
    //    public object range;
    //    public SourceLocation loc;
    //}
    //public class RawToken
    //{
    //    public Token type;
    //    public object value;
    //    public string pattern;
    //    public string flags;
    //    public object regex;
    //    public bool octal;
    //    public string cooked;
    //    public bool head;
    //    public bool tail;
    //    public int lineNumber;
    //    public int lineStart;
    //    public int start;
    //    public int end;
    //}
    //public class ScannerState
    //{
    //    public int index;
    //    public int lineNumber;
    //    public int lineStart;
    //    public List<string> curlyStack;
    //}
    //public class Scanner
    //{
    //    readonly string Source;
    //    readonly ErrorHandler ErrorHandler;
    //    bool TrackComment;
    //    bool IsModule;
    //    int Index;
    //    int LineNumber;
    //    int LineStart;
    //    List<string> CurlyStack;
    //    private readonly int Length;
    //    public Scanner(string code, ErrorHandler handler)
    //    {
    //        this.Source = code;
    //        this.ErrorHandler = handler;
    //        this.TrackComment = false;
    //        this.IsModule = false;
    //        this.Length = code.Length;
    //        this.Index = 0;
    //        this.LineNumber = (code.Length > 0) ? 1 : 0;
    //        this.LineStart = 0;
    //        this.CurlyStack = new[];
    //    }
    //    public ScannerState SaveState()
    //    {
    //        return new
    //        {
    //            index = this.Index,
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            curlyStack = this.CurlyStack.Slice()
    //        };
    //    }
    //    public void RestoreState(ScannerState state)
    //    {
    //        this.Index = state.Index;
    //        this.LineNumber = state.LineNumber;
    //        this.LineStart = state.LineStart;
    //        this.CurlyStack = state.CurlyStack;
    //    }
    //    public bool Eof()
    //    {
    //        return this.Index >= this.Length;
    //    }
    //    public object ThrowUnexpectedToken(void message)
    //    {
    //        return this.ErrorHandler.ThrowError(this.Index, this.LineNumber, this.Index - this.LineStart + 1, message);
    //    }
    //    private void TolerateUnexpectedToken(void message)
    //    {
    //        this.ErrorHandler.TolerateError(this.Index, this.LineNumber, this.Index - this.LineStart + 1, message);
    //    }
    //    // https://tc39.github.io/ecma262/#sec-comments
    //    private List<Comment> SkipSingleLineComment(int offset)
    //    {
    //        var comments = new[];
    //        var start; var loc;
    //        if (this.TrackComment)
    //        {
    //            comments = new[];
    //            start = this.Index - offset;
    //            loc = new
    //            {
    //                start = new
    //                {
    //                    line = this.LineNumber,
    //                    column = this.Index - this.LineStart - offset
    //                },
    //                end = new { }
    //            };
    //        }
    //        while (!this.Eof())
    //        {
    //            var ch = this.Source[this.Index];
    //            ++this.Index;
    //            if (Character.IsLineTerminator(ch))
    //            {
    //                if (this.TrackComment)
    //                {
    //                    loc.End = new
    //                    {
    //                        line = this.LineNumber,
    //                        column = this.Index - this.LineStart - 1
    //                    };
    //                    var entry = new
    //                    {
    //                        multiLine = false,
    //                        slice = new[start + offset, this.Index - 1],
    //    range = new [start, this.Index - 1],
    //    loc = loc };
    //                    comments.Add(entry);
    //                }
    //                if (ch == 13 && this.Source[this.Index] == 10)
    //                {
    //                    ++this.Index;
    //                }
    //                ++this.LineNumber;
    //                this.LineStart = this.Index;
    //                return comments;
    //            }
    //        }
    //        if (this.TrackComment)
    //        {
    //            loc.End = new
    //            {
    //                line = this.LineNumber,
    //                column = this.Index - this.LineStart
    //            };
    //            var entry = new
    //            {
    //                multiLine = false,
    //                slice = new[start + offset, this.Index],
    //    range = new [start, this.Index],
    //    loc = loc };
    //            comments.Add(entry);
    //        }
    //        return comments;
    //    }
    //    private List<Comment> SkipMultiLineComment()
    //    {
    //        var comments = new[];
    //        var start; var loc;
    //        if (this.TrackComment)
    //        {
    //            comments = new[];
    //            start = this.Index - 2;
    //            loc = new
    //            {
    //                start = new
    //                {
    //                    line = this.LineNumber,
    //                    column = this.Index - this.LineStart - 2
    //                },
    //                end = new { }
    //            };
    //        }
    //        while (!this.Eof())
    //        {
    //            var ch = this.Source[this.Index];
    //            if (Character.IsLineTerminator(ch))
    //            {
    //                if (ch == 13 && this.Source[this.Index + 1] == 10)
    //                {
    //                    ++this.Index;
    //                }
    //                ++this.LineNumber;
    //                ++this.Index;
    //                this.LineStart = this.Index;
    //            }
    //            else if (ch == 42)
    //            {
    //                // Block comment ends with '*/'.
    //                if (this.Source[this.Index + 1] == 47)
    //                {
    //                    this.Index += 2;
    //                    if (this.TrackComment)
    //                    {
    //                        loc.End = new
    //                        {
    //                            line = this.LineNumber,
    //                            column = this.Index - this.LineStart
    //                        };
    //                        var entry = new
    //                        {
    //                            multiLine = true,
    //                            slice = new[start + 2, this.Index - 2],
    //    range = new [start, this.Index],
    //    loc = loc };
    //                        comments.Add(entry);
    //                    }
    //                    return comments;
    //                }
    //                ++this.Index;
    //            }
    //            else
    //            {
    //                ++this.Index;
    //            }
    //        }
    //        // Ran off the end of the file - the whole thing is a comment
    //        if (this.TrackComment)
    //        {
    //            loc.End = new
    //            {
    //                line = this.LineNumber,
    //                column = this.Index - this.LineStart
    //            };
    //            var entry = new
    //            {
    //                multiLine = true,
    //                slice = new[start + 2, this.Index],
    //    range = new [start, this.Index],
    //    loc = loc };
    //            comments.Add(entry);
    //        }
    //        this.TolerateUnexpectedToken();
    //        return comments;
    //    }
    //    public void ScanComments()
    //    {
    //        var comments;
    //        if (this.TrackComment)
    //        {
    //            comments = new[];
    //        }
    //        var start = (this.Index == 0);
    //        while (!this.Eof())
    //        {
    //            var ch = this.Source[this.Index];
    //            if (Character.IsWhiteSpace(ch))
    //            {
    //                ++this.Index;
    //            }
    //            else if (Character.IsLineTerminator(ch))
    //            {
    //                ++this.Index;
    //                if (ch == 13 && this.Source[this.Index] == 10)
    //                {
    //                    ++this.Index;
    //                }
    //                ++this.LineNumber;
    //                this.LineStart = this.Index;
    //                start = true;
    //            }
    //            else if (ch == 47)
    //            { // U+002F is '/'
    //                ch = this.Source[this.Index + 1];
    //                if (ch == 47)
    //                {
    //                    this.Index += 2;
    //                    var comment = this.SkipSingleLineComment(2);
    //                    if (this.TrackComment)
    //                    {
    //                        comments = comments.Concat(comment);
    //                    }
    //                    start = true;
    //                }
    //                else if (ch == 42)
    //                { // U+002A is '*'
    //                    this.Index += 2;
    //                    var comment = this.SkipMultiLineComment();
    //                    if (this.TrackComment)
    //                    {
    //                        comments = comments.Concat(comment);
    //                    }
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }
    //            else if (start && ch == 45)
    //            { // U+002D is '-'
    //              // U+003E is '>'
    //                if ((this.Source[this.Index + 1] == 45) && (this.Source[this.Index + 2] == 62))
    //                {
    //                    // '-->' is a single-line comment
    //                    // '-->' is a single-line comment
    //                    this.Index += 3;
    //                    var comment = this.SkipSingleLineComment(3);
    //                    if (this.TrackComment)
    //                    {
    //                        comments = comments.Concat(comment);
    //                    }
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }
    //            else if (ch == 60 && !this.IsModule)
    //            { // U+003C is '<'
    //                if (this.Source.Slice(this.Index + 1, this.Index + 4) == "!--")
    //                {
    //                    this.Index += 4; // `<!--`
    //                    var comment = this.SkipSingleLineComment(4);
    //                    if (this.TrackComment)
    //                    {
    //                        comments = comments.Concat(comment);
    //                    }
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //        return comments;
    //    }
    //    // https://tc39.github.io/ecma262/#sec-future-reserved-words
    //    public bool IsFutureReservedWord(string id)
    //    {
    //        switch (id)
    //        {
    //            case "enum":
    //            case "export":
    //            case "import":
    //            case "super":
    //                return true;
    //            default:
    //                return false;
    //        }
    //    }
    //    public bool IsStrictModeReservedWord(string id)
    //    {
    //        switch (id)
    //        {
    //            case "implements":
    //            case "interface":
    //            case "package":
    //            case "private":
    //            case "protected":
    //            case "public":
    //            case "static":
    //            case "yield":
    //            case "let":
    //                return true;
    //            default:
    //                return false;
    //        }
    //    }
    //    public bool IsRestrictedWord(string id)
    //    {
    //        return id == "eval" || id == "arguments";
    //    }
    //    // https://tc39.github.io/ecma262/#sec-keywords
    //    private bool IsKeyword(string id)
    //    {
    //        switch (id.Length)
    //        {
    //            case 2:
    //                return (id == "if") || (id == "in") || (id == "do");
    //            case 3:
    //                return (id == "var") || (id == "for") || (id == "new") ||
    //                    (id == "try") || (id == "let");
    //            case 4:
    //                return (id == "this") || (id == "else") || (id == "case") ||
    //                    (id == "void") || (id == "with") || (id == "enum");
    //            case 5:
    //                return (id == "while") || (id == "break") || (id == "catch") ||
    //                    (id == "throw") || (id == "const") || (id == "yield") ||
    //                    (id == "class") || (id == "super");
    //            case 6:
    //                return (id == "return") || (id == "typeof") || (id == "delete") ||
    //                    (id == "switch") || (id == "export") || (id == "import");
    //            case 7:
    //                return (id == "default") || (id == "finally") || (id == "extends");
    //            case 8:
    //                return (id == "function") || (id == "continue") || (id == "debugger");
    //            case 10:
    //                return (id == "instanceof");
    //            default:
    //                return false;
    //        }
    //    }
    //    private int CodePointAt(int i)
    //    {
    //        var cp = this.Source[i];
    //        if (cp >= 55296 && cp <= 56319)
    //        {
    //            var second = this.Source[i + 1];
    //            if (second >= 56320 && second <= 57343)
    //            {
    //                var first = cp;
    //                cp = (first - 55296) * 1024 + second - 56320 + 65536;
    //            }
    //        }
    //        return cp;
    //    }
    //    private object ScanHexEscape(string prefix)
    //    {
    //        var len = (prefix == "u") ? 4 : 2;
    //        var code = 0;
    //        for (let i = 0; i < len; ++i)
    //        {
    //            if (!this.Eof() && Character.IsHexDigit(this.Source[this.Index]))
    //            {
    //                code = code * 16 + HexValue(this.Source[this.Index++]);
    //            }
    //            else
    //            {
    //                return null;
    //            }
    //        }
    //        return String.FromCharCode(code);
    //    }
    //    private string ScanUnicodeCodePointEscape()
    //    {
    //        var ch = this.Source[this.Index];
    //        var code = 0;
    //        // At least, one hex digit is required.
    //        if (ch == "}")
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        while (!this.Eof())
    //        {
    //            ch = this.Source[this.Index++];
    //            if (!Character.IsHexDigit(ch[0]))
    //            {
    //                break;
    //            }
    //            code = code * 16 + HexValue(ch);
    //        }
    //        if (code > 1114111 || ch != "}")
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        return Character.FromCodePoint(code);
    //    }
    //    private string GetIdentifier()
    //    {
    //        var start = this.Index++;
    //        while (!this.Eof())
    //        {
    //            var ch = this.Source[this.Index];
    //            if (ch == 92)
    //            {
    //                // Blackslash (U+005C) marks Unicode escape sequence.
    //                // Blackslash (U+005C) marks Unicode escape sequence.
    //                this.Index = start;
    //                return this.GetComplexIdentifier();
    //            }
    //            else if (ch >= 55296 && ch < 57343)
    //            {
    //                // Need to handle surrogate pairs.
    //                // Need to handle surrogate pairs.
    //                this.Index = start;
    //                return this.GetComplexIdentifier();
    //            }
    //            if (Character.IsIdentifierPart(ch))
    //            {
    //                ++this.Index;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //        return this.Source.Slice(start, this.Index);
    //    }
    //    private string GetComplexIdentifier()
    //    {
    //        var cp = this.CodePointAt(this.Index);
    //        var id = Character.FromCodePoint(cp);
    //        this.Index += id.Length;
    //        var ch;
    //        if (cp == 92)
    //        {
    //            if (this.Source[this.Index] != 117)
    //            {
    //                this.ThrowUnexpectedToken();
    //            }
    //            ++this.Index;
    //            if (this.Source[this.Index] == "{")
    //            {
    //                ++this.Index;
    //                ch = this.ScanUnicodeCodePointEscape();
    //            }
    //            else
    //            {
    //                ch = this.ScanHexEscape("u");
    //                if (ch == null || ch == "\\" || !Character.IsIdentifierStart(ch[0]))
    //                {
    //                    this.ThrowUnexpectedToken();
    //                }
    //            }
    //            id = ch;
    //        }
    //        while (!this.Eof())
    //        {
    //            cp = this.CodePointAt(this.Index);
    //            if (!Character.IsIdentifierPart(cp))
    //            {
    //                break;
    //            }
    //            ch = Character.FromCodePoint(cp);
    //            id += ch;
    //            this.Index += ch.Length;
    //            // '\u' (U+005C, U+0075) denotes an escaped character.
    //            if (cp == 92)
    //            {
    //                id = id.Substr(0, id.Length - 1);
    //                if (this.Source[this.Index] != 117)
    //                {
    //                    this.ThrowUnexpectedToken();
    //                }
    //                ++this.Index;
    //                if (this.Source[this.Index] == "{")
    //                {
    //                    ++this.Index;
    //                    ch = this.ScanUnicodeCodePointEscape();
    //                }
    //                else
    //                {
    //                    ch = this.ScanHexEscape("u");
    //                    if (ch == null || ch == "\\" || !Character.IsIdentifierPart(ch[0]))
    //                    {
    //                        this.ThrowUnexpectedToken();
    //                    }
    //                }
    //                id += ch;
    //            }
    //        }
    //        return id;
    //    }
    //    private void OctalToDecimal(string ch)
    //    {
    //        var octal = (ch != "0");
    //        var code = OctalValue(ch);
    //        if (!this.Eof() && Character.IsOctalDigit(this.Source[this.Index]))
    //        {
    //            octal = true;
    //            code = code * 8 + OctalValue(this.Source[this.Index++]);
    //            // 3 digits are only allowed when string starts
    //            // with 0, 1, 2, 3
    //            if ("0123".IndexOf(ch) >= 0 && !this.Eof() && Character.IsOctalDigit(this.Source[this.Index]))
    //            {
    //                code = code * 8 + OctalValue(this.Source[this.Index++]);
    //            }
    //        }
    //        return new
    //        {
    //            code = code,
    //            octal = octal
    //        };
    //    }
    //    // https://tc39.github.io/ecma262/#sec-names-and-keywords
    //    private RawToken ScanIdentifier()
    //    {
    //        var type;
    //        var start = this.Index;
    //        var id = (this.Source[start] == 92) ? this.GetComplexIdentifier() : this.GetIdentifier();
    //        // There is no keyword or literal with only one character.
    //        // Thus, it must be an identifier.
    //        if (id.Length == 1)
    //        {
    //            type = Token.Identifier;
    //        }
    //        else if (this.IsKeyword(id))
    //        {
    //            type = Token.Keyword;
    //        }
    //        else if (id == "null")
    //        {
    //            type = Token.NullLiteral;
    //        }
    //        else if (id == "true" || id == "false")
    //        {
    //            type = Token.BooleanLiteral;
    //        }
    //        else
    //        {
    //            type = Token.Identifier;
    //        }
    //        if (type != Token.Identifier && (start + id.Length != this.Index))
    //        {
    //            var restore = this.Index;
    //            this.Index = start;
    //            this.TolerateUnexpectedToken(Messages.InvalidEscapedReservedWord);
    //            this.Index = restore;
    //        }
    //        return new
    //        {
    //            type = type,
    //            value = id,
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    // https://tc39.github.io/ecma262/#sec-punctuators
    //    private RawToken ScanPunctuator()
    //    {
    //        var start = this.Index;
    //        var str = this.Source[this.Index];
    //        switch (str)
    //        {
    //            case "(":
    //            case "{":
    //                if (str == "{")
    //                {
    //                    this.CurlyStack.Add("{");
    //                }
    //                ++this.Index;
    //                break;
    //            case ".":
    //                ++this.Index;
    //                if (this.Source[this.Index] == "." && this.Source[this.Index + 1] == ".")
    //                {
    //                    // Spread operator: ...
    //                    // Spread operator: ...
    //                    this.Index += 2;
    //                    str = "...";
    //                }
    //                break;
    //            case "}":
    //                ++this.Index;
    //                this.CurlyStack.RemoveLast();
    //                break;
    //            case ")":
    //            case ";":
    //            case ",":
    //            case "[":
    //            case "]":
    //            case ":":
    //            case "?":
    //            case "~":
    //                ++this.Index;
    //                break;
    //            default:
    //                // 4-character punctuator.
    //                str = this.Source.Substr(this.Index, 4);
    //                if (str == ">>>=")
    //                {
    //                    this.Index += 4;
    //                }
    //                else
    //                {
    //                    // 3-character punctuators.
    //                    str = str.Substr(0, 3);
    //                    if (str == "===" || str == "!==" || str == ">>>" || str == "<<=" || str == ">>=" || str == "**=")
    //                    {
    //                        this.Index += 3;
    //                    }
    //                    else
    //                    {
    //                        // 2-character punctuators.
    //                        str = str.Substr(0, 2);
    //                        if (str == "&&" || str == "||" || str == "==" || str == "!=" || str == "+=" || str == "-=" || str == "*=" || str == "/=" || str == "++" || str == "--" || str == "<<" || str == ">>" || str == "&=" || str == "|=" || str == "^=" || str == "%=" || str == "<=" || str == ">=" || str == "=>" || str == "**")
    //                        {
    //                            this.Index += 2;
    //                        }
    //                        else
    //                        {
    //                            // 1-character punctuators.
    //                            str = this.Source[this.Index];
    //                            if ("<>=!+-*%&|^/".IndexOf(str) >= 0)
    //                            {
    //                                ++this.Index;
    //                            }
    //                        }
    //                    }
    //                }
    //        }
    //        if (this.Index == start)
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        return new
    //        {
    //            type = Token.Punctuator,
    //            value = str,
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    // https://tc39.github.io/ecma262/#sec-literals-numeric-literals
    //    private RawToken ScanHexLiteral(int start)
    //    {
    //        var num = "";
    //        while (!this.Eof())
    //        {
    //            if (!Character.IsHexDigit(this.Source[this.Index]))
    //            {
    //                break;
    //            }
    //            num += this.Source[this.Index++];
    //        }
    //        if (num.Length == 0)
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        if (Character.IsIdentifierStart(this.Source[this.Index]))
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        return new
    //        {
    //            type = Token.NumericLiteral,
    //            value = ParseInt("0x" + num, 16),
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    private RawToken ScanBinaryLiteral(int start)
    //    {
    //        var num = "";
    //        var ch;
    //        while (!this.Eof())
    //        {
    //            ch = this.Source[this.Index];
    //            if (ch != "0" && ch != "1")
    //            {
    //                break;
    //            }
    //            num += this.Source[this.Index++];
    //        }
    //        if (num.Length == 0)
    //        {
    //            // only 0b or 0B
    //            // only 0b or 0B
    //            this.ThrowUnexpectedToken();
    //        }
    //        if (!this.Eof())
    //        {
    //            ch = this.Source[this.Index];
    //            /* istanbul ignore else */
    //            if (Character.IsIdentifierStart(ch) || Character.IsDecimalDigit(ch))
    //            {
    //                this.ThrowUnexpectedToken();
    //            }
    //        }
    //        return new
    //        {
    //            type = Token.NumericLiteral,
    //            value = ParseInt(num, 2),
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    private RawToken ScanOctalLiteral(string prefix, int start)
    //    {
    //        var num = "";
    //        var octal = false;
    //        if (Character.IsOctalDigit(prefix[0]))
    //        {
    //            octal = true;
    //            num = "0" + this.Source[this.Index++];
    //        }
    //        else
    //        {
    //            ++this.Index;
    //        }
    //        while (!this.Eof())
    //        {
    //            if (!Character.IsOctalDigit(this.Source[this.Index]))
    //            {
    //                break;
    //            }
    //            num += this.Source[this.Index++];
    //        }
    //        if (!octal && num.Length == 0)
    //        {
    //            // only 0o or 0O
    //            // only 0o or 0O
    //            this.ThrowUnexpectedToken();
    //        }
    //        if (Character.IsIdentifierStart(this.Source[this.Index]) || Character.IsDecimalDigit(this.Source[this.Index]))
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        return new
    //        {
    //            type = Token.NumericLiteral,
    //            value = ParseInt(num, 8),
    //            octal = octal,
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    private bool IsImplicitOctalLiteral()
    //    {
    //        // Implicit octal, unless there is a non-octal digit.
    //        // (Annex B.1.1 on Numeric Literals)
    //        for (let i = this.Index + 1; i < this.Length; ++i)
    //        {
    //            var ch = this.Source[i];
    //            if (ch == "8" || ch == "9")
    //            {
    //                return false;
    //            }
    //            if (!Character.IsOctalDigit(ch[0]))
    //            {
    //                return true;
    //            }
    //        }
    //        return true;
    //    }
    //    private RawToken ScanNumericLiteral()
    //    {
    //        var start = this.Index;
    //        var ch = this.Source[start];
    //        Assert(Character.IsDecimalDigit(ch[0]) || (ch == "."), "Numeric literal must start with a decimal digit or a decimal point");
    //        var num = "";
    //        if (ch != ".")
    //        {
    //            num = this.Source[this.Index++];
    //            ch = this.Source[this.Index];
    //            // Hex number starts with '0x'.
    //            // Octal number starts with '0'.
    //            // Octal number in ES6 starts with '0o'.
    //            // Binary number in ES6 starts with '0b'.
    //            if (num == "0")
    //            {
    //                if (ch == "x" || ch == "X")
    //                {
    //                    ++this.Index;
    //                    return this.ScanHexLiteral(start);
    //                }
    //                if (ch == "b" || ch == "B")
    //                {
    //                    ++this.Index;
    //                    return this.ScanBinaryLiteral(start);
    //                }
    //                if (ch == "o" || ch == "O")
    //                {
    //                    return this.ScanOctalLiteral(ch, start);
    //                }
    //                if (ch && Character.IsOctalDigit(ch[0]))
    //                {
    //                    if (this.IsImplicitOctalLiteral())
    //                    {
    //                        return this.ScanOctalLiteral(ch, start);
    //                    }
    //                }
    //            }
    //            while (Character.IsDecimalDigit(this.Source[this.Index]))
    //            {
    //                num += this.Source[this.Index++];
    //            }
    //            ch = this.Source[this.Index];
    //        }
    //        if (ch == ".")
    //        {
    //            num += this.Source[this.Index++];
    //            while (Character.IsDecimalDigit(this.Source[this.Index]))
    //            {
    //                num += this.Source[this.Index++];
    //            }
    //            ch = this.Source[this.Index];
    //        }
    //        if (ch == "e" || ch == "E")
    //        {
    //            num += this.Source[this.Index++];
    //            ch = this.Source[this.Index];
    //            if (ch == "+" || ch == "-")
    //            {
    //                num += this.Source[this.Index++];
    //            }
    //            if (Character.IsDecimalDigit(this.Source[this.Index]))
    //            {
    //                while (Character.IsDecimalDigit(this.Source[this.Index]))
    //                {
    //                    num += this.Source[this.Index++];
    //                }
    //            }
    //            else
    //            {
    //                this.ThrowUnexpectedToken();
    //            }
    //        }
    //        if (Character.IsIdentifierStart(this.Source[this.Index]))
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        return new
    //        {
    //            type = Token.NumericLiteral,
    //            value = ParseFloat(num),
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    // https://tc39.github.io/ecma262/#sec-literals-string-literals
    //    private RawToken ScanStringLiteral()
    //    {
    //        var start = this.Index;
    //        var quote = this.Source[start];
    //        Assert((quote == "'" || quote == "\""), "String literal must starts with a quote");
    //        ++this.Index;
    //        var octal = false;
    //        var str = "";
    //        while (!this.Eof())
    //        {
    //            var ch = this.Source[this.Index++];
    //            if (ch == quote)
    //            {
    //                quote = "";
    //                break;
    //            }
    //            else if (ch == "\\")
    //            {
    //                ch = this.Source[this.Index++];
    //                if (!ch || !Character.IsLineTerminator(ch[0]))
    //                {
    //                    switch (ch)
    //                    {
    //                        case "u":
    //                            if (this.Source[this.Index] == "{")
    //                            {
    //                                ++this.Index;
    //                                str += this.ScanUnicodeCodePointEscape();
    //                            }
    //                            else
    //                            {
    //                                var unescapedChar = this.ScanHexEscape(ch);
    //                                if (unescapedChar == null)
    //                                {
    //                                    this.ThrowUnexpectedToken();
    //                                }
    //                                str += unescapedChar;
    //                            }
    //                            break;
    //                        case "x":
    //                            var unescaped = this.ScanHexEscape(ch);
    //                            if (unescaped == null)
    //                            {
    //                                this.ThrowUnexpectedToken(Messages.InvalidHexEscapeSequence);
    //                            }
    //                            str += unescaped;
    //                            break;
    //                        case "n":
    //                            str += "\n";
    //                            break;
    //                        case "r":
    //                            str += "\r";
    //                            break;
    //                        case "t":
    //                            str += "\t";
    //                            break;
    //                        case "b":
    //                            str += "\b";
    //                            break;
    //                        case "f":
    //                            str += "\f";
    //                            break;
    //                        case "v":
    //                            str += "\v";
    //                            break;
    //                        case "8":
    //                        case "9":
    //                            str += ch;
    //                            this.TolerateUnexpectedToken();
    //                            break;
    //                        default:
    //                            if (ch && Character.IsOctalDigit(ch[0]))
    //                            {
    //                                var octToDec = this.OctalToDecimal(ch);
    //                                octal = octToDec.Octal || octal;
    //                                str += String.FromCharCode(octToDec.Code);
    //                            }
    //                            else
    //                            {
    //                                str += ch;
    //                            }
    //                            break;
    //                    }
    //                }
    //                else
    //                {
    //                    ++this.LineNumber;
    //                    if (ch == "\r" && this.Source[this.Index] == "\n")
    //                    {
    //                        ++this.Index;
    //                    }
    //                    this.LineStart = this.Index;
    //                }
    //            }
    //            else if (Character.IsLineTerminator(ch[0]))
    //            {
    //                break;
    //            }
    //            else
    //            {
    //                str += ch;
    //            }
    //        }
    //        if (quote != "")
    //        {
    //            this.Index = start;
    //            this.ThrowUnexpectedToken();
    //        }
    //        return new
    //        {
    //            type = Token.StringLiteral,
    //            value = str,
    //            octal = octal,
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    // https://tc39.github.io/ecma262/#sec-template-literal-lexical-components
    //    private RawToken ScanTemplate()
    //    {
    //        var cooked = "";
    //        var terminated = false;
    //        var start = this.Index;
    //        var head = (this.Source[start] == "`");
    //        var tail = false;
    //        var rawOffset = 2;
    //        ++this.Index;
    //        while (!this.Eof())
    //        {
    //            var ch = this.Source[this.Index++];
    //            if (ch == "`")
    //            {
    //                rawOffset = 1;
    //                tail = true;
    //                terminated = true;
    //                break;
    //            }
    //            else if (ch == "$")
    //            {
    //                if (this.Source[this.Index] == "{")
    //                {
    //                    this.CurlyStack.Add("${");
    //                    ++this.Index;
    //                    terminated = true;
    //                    break;
    //                }
    //                cooked += ch;
    //            }
    //            else if (ch == "\\")
    //            {
    //                ch = this.Source[this.Index++];
    //                if (!Character.IsLineTerminator(ch[0]))
    //                {
    //                    switch (ch)
    //                    {
    //                        case "n":
    //                            cooked += "\n";
    //                            break;
    //                        case "r":
    //                            cooked += "\r";
    //                            break;
    //                        case "t":
    //                            cooked += "\t";
    //                            break;
    //                        case "u":
    //                            if (this.Source[this.Index] == "{")
    //                            {
    //                                ++this.Index;
    //                                cooked += this.ScanUnicodeCodePointEscape();
    //                            }
    //                            else
    //                            {
    //                                var restore = this.Index;
    //                                var unescapedChar = this.ScanHexEscape(ch);
    //                                if (unescapedChar != null)
    //                                {
    //                                    cooked += unescapedChar;
    //                                }
    //                                else
    //                                {
    //                                    this.Index = restore;
    //                                    cooked += ch;
    //                                }
    //                            }
    //                            break;
    //                        case "x":
    //                            var unescaped = this.ScanHexEscape(ch);
    //                            if (unescaped == null)
    //                            {
    //                                this.ThrowUnexpectedToken(Messages.InvalidHexEscapeSequence);
    //                            }
    //                            cooked += unescaped;
    //                            break;
    //                        case "b":
    //                            cooked += "\b";
    //                            break;
    //                        case "f":
    //                            cooked += "\f";
    //                            break;
    //                        case "v":
    //                            cooked += "\v";
    //                            break;
    //                        default:
    //                            if (ch == "0")
    //                            {
    //                                if (Character.IsDecimalDigit(this.Source[this.Index]))
    //                                {
    //                                    // Illegal: \01 \02 and so on
    //                                    // Illegal: \01 \02 and so on
    //                                    this.ThrowUnexpectedToken(Messages.TemplateOctalLiteral);
    //                                }
    //                                cooked += "\0";
    //                            }
    //                            else if (Character.IsOctalDigit(ch[0]))
    //                            {
    //                                // Illegal: \1 \2
    //                                // Illegal: \1 \2
    //                                this.ThrowUnexpectedToken(Messages.TemplateOctalLiteral);
    //                            }
    //                            else
    //                            {
    //                                cooked += ch;
    //                            }
    //                            break;
    //                    }
    //                }
    //                else
    //                {
    //                    ++this.LineNumber;
    //                    if (ch == "\r" && this.Source[this.Index] == "\n")
    //                    {
    //                        ++this.Index;
    //                    }
    //                    this.LineStart = this.Index;
    //                }
    //            }
    //            else if (Character.IsLineTerminator(ch[0]))
    //            {
    //                ++this.LineNumber;
    //                if (ch == "\r" && this.Source[this.Index] == "\n")
    //                {
    //                    ++this.Index;
    //                }
    //                this.LineStart = this.Index;
    //                cooked += "\n";
    //            }
    //            else
    //            {
    //                cooked += ch;
    //            }
    //        }
    //        if (!terminated)
    //        {
    //            this.ThrowUnexpectedToken();
    //        }
    //        if (!head)
    //        {
    //            this.CurlyStack.RemoveLast();
    //        }
    //        return new
    //        {
    //            type = Token.Template,
    //            value = this.Source.Slice(start + 1, this.Index - rawOffset),
    //            cooked = cooked,
    //            head = head,
    //            tail = tail,
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    // https://tc39.github.io/ecma262/#sec-literals-regular-expression-literals
    //    private object TestRegExp(string pattern, string flags)
    //    {
    //        var astralSubstitute = "\uFFFF";
    //        var tmp = pattern;
    //        if (flags.IndexOf("u") >= 0)
    //        {
    //            tmp = tmp.Replace(/\\u\{ ([0 - 9a - fA - F] +)\}|\\u([a - fA - F0 - 9]{ 4})/ g, (void $0, void $1, void $2) => {
    //                var codePoint = ParseInt($1 || $2, 16);
    //                if (codePoint > 1114111)
    //                {
    //                    this.ThrowUnexpectedToken(Messages.InvalidRegExp);
    //                }
    //                if (codePoint <= 65535)
    //                {
    //                    return String.FromCharCode(codePoint);
    //                }
    //                return astralSubstitute;
    //            }).Replace(/[\uD800-\uDBFF][\uDC00-\uDFFF] / g, astralSubstitute);
    //        }
    //        // First, detect invalid regular expressions.
    //        try
    //        {
    //            RegExp(tmp);
    //        }
    //        catch (e)
    //        {
    //            this.ThrowUnexpectedToken(Messages.InvalidRegExp);
    //        }
    //        // Return a regular expression object for this pattern-flag pair, or
    //        // `null` in case the current environment doesn't support the flags it
    //        // uses.
    //        try
    //        {
    //            return new RegExp(pattern, flags);
    //        }
    //        catch (exception)
    //        {
    //            /* istanbul ignore next */
    //            return null;
    //        }
    //    }
    //    private string ScanRegExpBody()
    //    {
    //        var ch = this.Source[this.Index];
    //        Assert(ch == "/", "Regular expression literal must start with a slash");
    //        var str = this.Source[this.Index++];
    //        var classMarker = false;
    //        var terminated = false;
    //        while (!this.Eof())
    //        {
    //            ch = this.Source[this.Index++];
    //            str += ch;
    //            if (ch == "\\")
    //            {
    //                ch = this.Source[this.Index++];
    //                // https://tc39.github.io/ecma262/#sec-literals-regular-expression-literals
    //                if (Character.IsLineTerminator(ch[0]))
    //                {
    //                    this.ThrowUnexpectedToken(Messages.UnterminatedRegExp);
    //                }
    //                str += ch;
    //            }
    //            else if (Character.IsLineTerminator(ch[0]))
    //            {
    //                this.ThrowUnexpectedToken(Messages.UnterminatedRegExp);
    //            }
    //            else if (classMarker)
    //            {
    //                if (ch == "]")
    //                {
    //                    classMarker = false;
    //                }
    //            }
    //            else
    //            {
    //                if (ch == "/")
    //                {
    //                    terminated = true;
    //                    break;
    //                }
    //                else if (ch == "[")
    //                {
    //                    classMarker = true;
    //                }
    //            }
    //        }
    //        if (!terminated)
    //        {
    //            this.ThrowUnexpectedToken(Messages.UnterminatedRegExp);
    //        }
    //        // Exclude leading and trailing slash.
    //        return str.Substr(1, str.Length - 2);
    //    }
    //    private string ScanRegExpFlags()
    //    {
    //        var str = "";
    //        var flags = "";
    //        while (!this.Eof())
    //        {
    //            var ch = this.Source[this.Index];
    //            if (!Character.IsIdentifierPart(ch[0]))
    //            {
    //                break;
    //            }
    //            ++this.Index;
    //            if (ch == "\\" && !this.Eof())
    //            {
    //                ch = this.Source[this.Index];
    //                if (ch == "u")
    //                {
    //                    ++this.Index;
    //                    var restore = this.Index;
    //                    var char = this.ScanHexEscape("u");
    //                    if (char != null)
    //                    {
    //                        flags += char;
    //                        for (str += "\\u"; restore < this.Index; ++restore)
    //                        {
    //                            str += this.Source[restore];
    //                        }
    //                    }
    //                    else
    //                    {
    //                        this.Index = restore;
    //                        flags += "u";
    //                        str += "\\u";
    //                    }
    //                    this.TolerateUnexpectedToken();
    //                }
    //                else
    //                {
    //                    str += "\\";
    //                    this.TolerateUnexpectedToken();
    //                }
    //            }
    //            else
    //            {
    //                flags += ch;
    //                str += ch;
    //            }
    //        }
    //        return flags;
    //    }
    //    public RawToken ScanRegExp()
    //    {
    //        var start = this.Index;
    //        var pattern = this.ScanRegExpBody();
    //        var flags = this.ScanRegExpFlags();
    //        var value = this.TestRegExp(pattern, flags);
    //        return new
    //        {
    //            type = Token.RegularExpression,
    //            value = "",
    //            pattern = pattern,
    //            flags = flags,
    //            regex = value,
    //            lineNumber = this.LineNumber,
    //            lineStart = this.LineStart,
    //            start = start,
    //            end = this.Index
    //        };
    //    }
    //    public RawToken Lex()
    //    {
    //        if (this.Eof())
    //        {
    //            return new
    //            {
    //                type = Token.EOF,
    //                value = "",
    //                lineNumber = this.LineNumber,
    //                lineStart = this.LineStart,
    //                start = this.Index,
    //                end = this.Index
    //            };
    //        }
    //        var cp = this.Source[this.Index];
    //        if (Character.IsIdentifierStart(cp))
    //        {
    //            return this.ScanIdentifier();
    //        }
    //        // Very common: ( and ) and ;
    //        if (cp == 40 || cp == 41 || cp == 59)
    //        {
    //            return this.ScanPunctuator();
    //        }
    //        // String literal starts with single quote (U+0027) or double quote (U+0022).
    //        if (cp == 39 || cp == 34)
    //        {
    //            return this.ScanStringLiteral();
    //        }
    //        // Dot (.) U+002E can also start a floating-point number, hence the need
    //        // to check the next character.
    //        if (cp == 46)
    //        {
    //            if (Character.IsDecimalDigit(this.Source[this.Index + 1]))
    //            {
    //                return this.ScanNumericLiteral();
    //            }
    //            return this.ScanPunctuator();
    //        }
    //        if (Character.IsDecimalDigit(cp))
    //        {
    //            return this.ScanNumericLiteral();
    //        }
    //        // Template literals start with ` (U+0060) for template head
    //        // or } (U+007D) for template middle or template tail.
    //        if (cp == 96 || (cp == 125 && this.CurlyStack[this.CurlyStack.Length - 1] == "${"))
    //        {
    //            return this.ScanTemplate();
    //        }
    //        // Possible identifier start in a surrogate pair.
    //        if (cp >= 55296 && cp < 57343)
    //        {
    //            if (Character.IsIdentifierStart(this.CodePointAt(this.Index)))
    //            {
    //                return this.ScanIdentifier();
    //            }
    //        }
    //        return this.ScanPunctuator();
    //    }
    //}
}
