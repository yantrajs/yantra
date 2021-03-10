namespace YantraJS.Core.FastParser
{
    public class FastToken
    {
        public static FastToken Empty;

        public readonly TokenTypes Type;

        public readonly StringSpan Span;

        public FastToken(TokenTypes type, string source, int start, int length)
        {
            this.Type = type;
            this.Span = new StringSpan(source, start, length);
        }

    }
}
