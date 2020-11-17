#nullable enable
using System;
using System.IO;

namespace YantraJS.Core
{
    public class StringSpanReader : TextReader
    {
        readonly StringSpan span;
        private int index = 0;
        public StringSpanReader(in StringSpan span)
        {
            this.span = span;
        }

        public override int Peek()
        {
            if (index >= span.Length)
                return -1;
            return span[index];
        }

        public override int Read()
        {
            if (index >= span.Length)
                return -1;
            return span[index++];
        }

        public override string ReadToEnd()
        {
            return span.Substring(index).Value ?? string.Empty;
        }
    }
}
