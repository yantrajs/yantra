using System;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class FastToken
    {
        public static FastToken Empty;

        public readonly TokenTypes Type;
        public readonly StringSpan Span;
        public readonly double Number;
        public readonly string CookedText;
        public readonly string Flags;
        public readonly bool IsKeyword;
        public readonly FastKeywords Keyword;
        public readonly FastKeywords ContextualKeyword;

        public readonly SpanLocation Start;
        public readonly SpanLocation End;

        public FastToken Next;
        public FastToken Previous;

        ///// <summary>
        ///// Marks current token ends with line
        ///// </summary>
        //public readonly bool LineTerminator;

        public FastToken AsString()
        {
            return new FastToken(
                TokenTypes.String,
                Span.Source,
                CookedText ?? Span.Value,
                Flags, Span.Offset,
                Span.Length, Start, End, ContextualKeyword);
        }

        private FastToken(
            TokenTypes type,
            string source = null,
            string cooked = null,
            string flags = null,
            int start = 0,
            int length = 0,
            in SpanLocation startLocation = default,
            in SpanLocation endLocation = default,
            FastKeywords contextualKeyword = FastKeywords.none)
        {
            this.Type = type;
            this.Start = startLocation;
            this.End = endLocation;
            this.Span = new StringSpan(source, start, Math.Min(source.Length - start, length));
            this.CookedText = cooked;
            this.Flags = flags;
            this.Number = 0;
            this.ContextualKeyword = contextualKeyword;
        }

        public FastToken(
            TokenTypes type, 
            string source = null,
            string cooked = null,
            string flags = null,
            int start = 0, 
            int length = 0,
            in SpanLocation startLocation = default,
            in SpanLocation endLocation = default,
            bool parseNumber = false,
            FastKeywordMap keywords = null)
        {
            this.Type = type;
            this.Start = startLocation;
            this.End = endLocation;
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

                        // contextual...

                    case FastKeywords.get:
                    case FastKeywords.set:
                    case FastKeywords.of:
                    case FastKeywords.constructor:
                    case FastKeywords.from:
                    case FastKeywords.@as:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = k;
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
