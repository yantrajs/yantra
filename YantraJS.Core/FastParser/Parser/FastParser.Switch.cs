using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public readonly struct AstCase
    {
        public readonly AstExpression Test;
        public readonly AstStatement[] Statements;

        public AstCase(AstExpression test, AstStatement[] last)
        {
            this.Test = test;
            this.Statements = last;
        }
    }

    partial class FastParser
    {



        bool Switch(out AstStatement node)
        {
            var begin = Location;
            stream.Consume();
            node = null;

            stream.Expect(TokenTypes.BracketStart);
            if (!Expression(out var target))
                throw stream.Unexpected();
            stream.Expect(TokenTypes.BracketEnd);

            stream.Expect(TokenTypes.CurlyBracketStart);
            var nodes = Pool.AllocateList<AstCase>();
            var statements = Pool.AllocateList<AstStatement>();
            AstStatement[] last = null;
            AstExpression test = null;
            try
            {
                while (!stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                {
                    if(stream.CheckAndConsume(FastKeywords.@case))
                    {
                        if(last != null)
                        {
                            last = statements.Release();
                            nodes.Add(new AstCase(test, last));
                        }
                        if (!Expression(out test))
                            throw stream.Unexpected();
                        stream.Expect(TokenTypes.Colon);
                    } else if(stream.CheckAndConsume(FastKeywords.@default))
                    {
                        test = null;
                        last = statements.Release();
                    }
                    if (Statement(out var stmt))
                        statements.Add(stmt);
                }

                if(last != null)
                {
                    nodes.Add(new AstCase(test, last));
                }

                node = new AstSwitchStatement(begin.Token, PreviousToken, target, nodes.Release());

            } finally
            {
                nodes.Clear();
                statements.Clear();
            }

            return false;
        }


    }

}
