﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace YantraJS.Core.FastParser
{
    public class FastScanner
    {
        public readonly StringSpan Text;

        public FastScanner(in StringSpan text)
        {
            this.Text = text;
        }
    }

    public delegate bool ParseToken(FastScanner scanner, out FastToken token);

    public readonly struct FastTokenType
    {
        public readonly int Id;
        public readonly ParseToken Parser;

        private static int nextId = 0;

        public FastTokenType(ParseToken parser)
        {
            this.Id = Interlocked.Increment(ref nextId);
            this.Parser = parser;
        }

        public static bool operator == (in FastTokenType left, in FastTokenType right)
        {
            return left.Id == right.Id;
        }
        public static bool operator !=(in FastTokenType left, in FastTokenType right)
        {
            return left.Id != right.Id;
        }

        public static implicit operator FastTokenType(ParseToken parser)
        {
            return new FastTokenType(parser);
        }

        public override bool Equals(object obj)
        {
            return obj is  FastTokenType ft ? Id == ft.Id : false;
        }

        public override int GetHashCode()
        {
            return Id;
        }

    }

    public static class TokenTypes
    {
        // public static FastTokenType Identifier = (ParseToken) (scanner, out bool success) => { };
    }

    public readonly struct FastToken
    {
        public readonly FastTokenType Type;

        public readonly StringSpan Span;

        public FastToken(FastTokenType type, StringSpan span)
        {
            this.Type = type;
            this.Span = span;
        }

        public FastToken(FastTokenType type, string source, int start, int length)
        {
            this.Type = type;
            this.Span = new StringSpan(source, start, length);
        }

    }

    public class FastNode
    {

    }

    public class FastExpression
    {

    }

    public class FastBinaryExpression
    {

    }

    public class FastStatement
    {

    }

    public class FastBlock
    {
        public List<FastStatement> Statements;
    }

    public class FastProgram
    {



    }
}