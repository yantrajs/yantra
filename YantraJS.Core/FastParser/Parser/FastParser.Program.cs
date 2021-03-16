using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public class AstNode
    {
        public readonly FastNodeType Type;
        public readonly FastToken Start;
        public readonly FastToken End;

        public AstNode(FastToken start, FastNodeType type, FastToken end)
        {
            this.Start = start;
            this.Type = type;
            this.End = end;
        }
    }

    public class AstProgram : AstNode
    {
        public AstProgram(FastToken token, SparseList<FastStatement> list, FastToken end)
            : base(token, FastNodeType.Program, end )
        {
        }
    }


    partial class FastParser
    {




        bool Program(out FastNode node)
        {
            var begin = Location;
            SparseList<FastStatement> list = new SparseList<FastStatement>();
            while(Statement(out var stmt))
            {
                list.Add(stmt);
            }
            if (stream.CheckAndConsume(TokenTypes.EOF)) {
                node = new AstProgram(begin.Token, list, PreviousToken);
                return true;
            }
            node = default;
            return begin.Reset(out node);
        }

        public StreamLocation Location => new StreamLocation(this, stream.Position, stream.Current);

        public FastToken PreviousToken => stream.Previous;

        public readonly struct StreamLocation
        {
            private readonly FastParser parser;
            private readonly int position;
            public readonly FastToken Token;

            public StreamLocation(FastParser parser, int index, FastToken token)
            {
                this.parser = parser;
                this.position = index;
                this.Token = token;
            }

            public bool Reset<T>(out T value)
            {
                parser.stream.Reset(position);
                value = default;
                return false;
            }
        }

    }

}
