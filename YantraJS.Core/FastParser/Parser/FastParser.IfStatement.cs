﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public class AstIfStatement : AstStatement
    {
        public readonly AstExpression Test;
        public readonly AstStatement True;
        public readonly AstStatement False;

        public AstIfStatement(FastToken start, FastToken end, AstExpression test, AstStatement @true, AstStatement @false = null)
            : base(start, FastNodeType.IfStatement, end)
        {
            this.Test = test;
            this.True = @true;
            this.False = @false;
        }
    }

    partial class FastParser
    {



        bool IfStatement(out AstStatement node)
        {
            var begin = Location;
            node = default;
            stream.Consume();

            stream.Expect(TokenTypes.BracketStart);
            if (!Expression(out var test))
                throw stream.Unexpected();

            stream.Expect(TokenTypes.BracketEnd);

            if (!Statement(out var @true))
                throw stream.Unexpected();

            if (stream.CheckAndConsume(FastKeywords.@else))
            {
                if (!Statement(out var @else))
                    throw stream.Unexpected();
                node = new AstIfStatement(begin.Token, PreviousToken, test, @true, @else);
                return true;
            }

            node = new AstIfStatement(begin.Token, PreviousToken, test, @true);
            return true;
        }


    }

}