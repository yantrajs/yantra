using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YantraJS.Core;

internal class CharReader: TextReader
{
    private readonly IEnumerator<char> reader;
    private bool done;
    private char peek;

    public CharReader(IEnumerable<char> reader)
    {
        this.reader = reader.GetEnumerator();
        this.done = this.reader.MoveNext();
        this.peek = this.reader.Current;
    }

    public override int Peek()
    {
        return peek;
    }

    public override int Read()
    {
        var r = this.peek;
        this.done = this.reader.MoveNext();
        this.peek = this.reader.Current;
        return r;
    }

}

