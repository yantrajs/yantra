using System;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class FastToken
    {
        public static FastToken Empty;

        public readonly TokenTypes Type;
        public readonly int StartLine;
        public readonly int StartColumn;
        public readonly StringSpan Span;
        public readonly double Number;
        public readonly string CookedText;
        public readonly string Flags;
        public readonly int EndLine;

        public readonly int EndColumn;
        public readonly bool IsKeyword;
        public readonly FastKeywords Keyword;
        public readonly FastKeywords ContextualKeyword;

        public SpanLocation StartLocation => new SpanLocation(StartLine, StartColumn);

        public SpanLocation EndLocation => new SpanLocation(EndLine, EndColumn);

        public FastToken(
            TokenTypes type, 
            string source, 
            string cooked,
            string flags,
            int start, 
            int length,
            int startLine,
            int startColumn,
            int endLine,
            int endColumn,
            bool parseNumber = false,
            FastKeywordMap keywords = null)
        {
            this.Type = type;
            this.StartLine = startLine;
            this.StartColumn = startColumn;
            this.EndLine = endLine;
            this.EndColumn = endColumn;
            this.Span = new StringSpan(source, start, Math.Min(source.Length-start, length));
            this.CookedText = cooked;
            this.Flags = flags;
            if (parseNumber) {
                this.Number = Utils.NumberParser.CoerceToNumber(Span);
            } else {
                this.Number = 0;
            }
            if (keywords != null)
            {
                this.IsKeyword = keywords.IsKeyword(Span, out var k);
                this.Keyword = k;

                switch(k)
                {
                    /*
                     * instnaceof is an operator used in binary expression
                     *
                     */
                    case FastKeywords.instanceof:
                        IsKeyword = false;
                        this.Keyword = FastKeywords.none;
                        Type = TokenTypes.InstanceOf;
                        break;
                    case FastKeywords.@in:
                        IsKeyword = false;
                        this.Keyword = FastKeywords.none;
                        Type = TokenTypes.In;
                        break;
                    case FastKeywords.@null:
                        IsKeyword = false;
                        Type = TokenTypes.Null;
                        this.Keyword = FastKeywords.none;
                        break;
                    case FastKeywords.@true:
                        IsKeyword = false;
                        Type = TokenTypes.True;
                        this.Keyword = FastKeywords.none;
                        break;
                    case FastKeywords.@false:
                        IsKeyword = false;
                        Type = TokenTypes.False;
                        this.Keyword = FastKeywords.none;
                        break;
                    case FastKeywords.get:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = FastKeywords.get;
                        this.Keyword = FastKeywords.none;
                        break;
                    case FastKeywords.set:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = FastKeywords.set;
                        this.Keyword = FastKeywords.none;
                        break;
                    case FastKeywords.of:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = FastKeywords.of;
                        this.Keyword = FastKeywords.none;
                        break;
                    case FastKeywords.constructor:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = FastKeywords.constructor;
                        this.Keyword = FastKeywords.none;
                        break;
                }
            }
        }

        public override string ToString()
        {
            return $"{Type} {Span}";
        }
    }
}
