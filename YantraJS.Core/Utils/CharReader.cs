using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YantraJS.Core;

internal class CharReader: TextReader
{
    private readonly IEnumerator<char> reader;
    private bool done;
    private int peek;

    public CharReader(IEnumerable<char> reader)
    {
        this.reader = reader.GetEnumerator();
        this.done = !this.reader.MoveNext();
        this.peek = this.reader.Current;
    }

    public override int Peek()
    {
        return peek;
    }

    public override int Read()
    {
        if (this.done)
        {
            return -1;
        }
        var r = this.peek;
        this.done = !this.reader.MoveNext();
        this.peek = this.done ? -1 : this.reader.Current;
        return r;
    }

    public override string ReadToEnd()
    {
        var sb = new StringBuilder();
        while (!this.done)
        {
            sb.Append((char)this.Read());
        }
        return sb.ToString();
    }

}

