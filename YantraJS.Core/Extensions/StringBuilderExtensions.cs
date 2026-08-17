using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core;

public static class StringBuilderExtensions
{

    public static StringBuilder AppendStringOrChar(this StringBuilder sb, JSValue value)
    {
        var sc = value.ToStringOrChar();
        return sb.Append(sc);
    }

    public static StringBuilder AppendStringOrChar(this StringBuilder sb, StringOrChar value)
    {
        return value.IsChar ? sb.Append(value.Char) : sb.Append(value.String);
    }

}

