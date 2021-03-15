using System;

namespace YantraJS.Core.FastParser
{
    public class FastParseException : Exception
    {

        public readonly FastToken Token;
        public FastParseException(FastToken token, string message): base(message)
        {
            Token = token;
        }
    }
}
