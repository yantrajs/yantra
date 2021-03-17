using System;

namespace YantraJS.Core.FastParser
{
    public class FastToken
    {
        public static FastToken Empty;

        public readonly TokenTypes Type;
        public readonly int StartLine;
        public readonly int StartColumn;
        public readonly StringSpan Span;

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
            int start, 
            int length,
            int startLine,
            int startColumn,
            int endLine,
            int endColumn,
            FastKeywordMap keywords = null)
        {
            this.Type = type;
            this.StartLine = startLine;
            this.StartColumn = startColumn;
            this.EndLine = endLine;
            this.EndColumn = endColumn;
            this.Span = new StringSpan(source, start, length);
            if(keywords != null)
            {
                this.IsKeyword = keywords.IsKeyword(Span, out var k);
                this.Keyword = k;

                switch(k)
                {
                    case FastKeywords.@null:
                        IsKeyword = false;
                        Type = TokenTypes.Null;
                        break;
                    case FastKeywords.@true:
                        IsKeyword = false;
                        Type = TokenTypes.True;
                        break;
                    case FastKeywords.@false:
                        IsKeyword = false;
                        Type = TokenTypes.False;
                        break;
                    case FastKeywords.get:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = FastKeywords.get;
                        break;
                    case FastKeywords.set:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = FastKeywords.set;
                        break;
                    case FastKeywords.of:
                        IsKeyword = false;
                        Type = TokenTypes.Identifier;
                        ContextualKeyword = FastKeywords.of;
                        break;
                }
            }
        }
    }
}
